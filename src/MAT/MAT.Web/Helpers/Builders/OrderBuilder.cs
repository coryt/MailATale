using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using MAT.Core.InputModels;
using MAT.Core.Models.Account;
using MAT.Core.Models.Enumerations;
using MAT.Core.Models.SignUp;
using MAT.Core.Models.Subscription;
using MAT.Core.Services;
using MAT.Web.Infrastructure.Common;
using Raven.Client;

namespace MAT.Web.Helpers.Builders
{
    public class OrderBuilder
    {
        private readonly IDocumentSession _ravenSession;
        private readonly ILogger _logger;
        private readonly ShippingCalculator _shippingCalculator;
        private readonly Dictionary<string, SubscriptionProduct> _products;
        private readonly List<PreferenceQuestion> _signupQuestions;

        public OrderBuilder(IDocumentSession ravenSession, ILogger logger, ShippingCalculator shippingCalculator)
        {
            _ravenSession = ravenSession;
            _logger = logger;
            _shippingCalculator = shippingCalculator;
            _products = _ravenSession.Query<SubscriptionProduct>().Where(p => p.Status == ProductStatus.Active).ToDictionary(p => p.Name == "Book Box Plus" ? "plus" : "basic", g => g, StringComparer.OrdinalIgnoreCase);
            _signupQuestions = ravenSession.GetSignupPreferenceQuestions();
        }

        public bool TryBuildOrder(SignUpModel model, User user, Promotion promotion, out SubscriptionOrder subscriptionOrder)
        {
            subscriptionOrder = null;
            try
            {
                var subscriptions = BuildSubscriptions(model.Lines, user.ShippingAddress, user.Id);

                subscriptionOrder = new SubscriptionOrder(user, user.BillingAddress, user.ShippingAddress, model.Payment, promotion, subscriptions, _shippingCalculator);
                _ravenSession.Store(subscriptionOrder);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Fatal("There was a problem with the order info (payment, subscription).", ex);
                return false;
            }
        }

        public bool TryBuildSubscription(User user, ReaderModel readerModel, SubscriptionProduct product, out UserSubscription subscription)
        {
            subscription = null;
            try
            {
                var reader = new Reader
                {
                    Name = readerModel.Name,
                    Birthdate = readerModel.GetBirthdate(),
                    Gender = readerModel.Gender,
                    Grade = readerModel.GradeLevel,
                    ReadingLevel = readerModel.ReadingLevel,
                    RelationshipToUser = readerModel.Relationship
                };

                reader.AddPreferenceResponse(_signupQuestions, readerModel.PreferenceAnswers);

                subscription = new UserSubscription
                {
                    SubscriptionStatus = SubscriptionStatus.PendingActivation,
                    StartedOn = DateTime.Now,
                    DeliveryFrequency = DeliveryFrequency.Monthly,
                    SubscriptionProduct = product,
                    Reader = reader,
                    ShippingAddress = user.ShippingAddress,
                    UserId = user.Id
                };

                _ravenSession.Store(subscription);

                return true;
            }
            catch (Exception e)
            {
                _logger.Fatal("There was a probelm with the subscription info.", e);
                subscription = null;
                return false;
            }
        }

        private List<UserSubscription> BuildSubscriptions(IEnumerable<SignUpLineItem> lineItems, Address shippingAddress, string userId)
        {
            var subscriptions = new List<UserSubscription>();
            foreach (var lineItem in lineItems)
            {
                var userSub = new UserSubscription
                                  {
                                      SubscriptionStatus = SubscriptionStatus.PendingActivation,
                                      StartedOn = DateTime.Now,
                                      DeliveryFrequency =
                                          lineItem.DeliverySchedule == "monthly"
                                              ? DeliveryFrequency.Monthly
                                              : DeliveryFrequency.BiMonthly,
                                      SubscriptionProduct = _products[lineItem.Subscription],
                                      Reader = lineItem.Reader.ToReader(_signupQuestions),
                                      ShippingAddress = shippingAddress,
                                      UserId = userId
                                  };
                _ravenSession.Store(userSub);
                subscriptions.Add(userSub);
            }
            return subscriptions;
        }
    }
}