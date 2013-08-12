using System;
using Castle.Core.Logging;
using Raven.Abstractions.Exceptions;
using Raven.Client;

namespace MAT.Core.Tasks
{
    public abstract class BackgroundTask
    {
        protected IDocumentSession DocumentSession;
        private  ILogger _logger;
        public bool RecurringTask { get; set; }

        protected virtual void Initialize(IDocumentSession session)
        {
            DocumentSession = session;
            DocumentSession.Advanced.UseOptimisticConcurrency = true;
        }

        protected virtual void OnError(Exception e)
        {
        }

        public bool? Run(IDocumentSession openSession)
        {
            Initialize(openSession);
            try
            {
                Execute();
                DocumentSession.SaveChanges();
                TaskExecutor.StartExecuting();
                return true;
            }
            catch (ConcurrencyException e)
            {
                _logger.Error("Could not execute task " + GetType().Name, e);
                OnError(e);
                return null;
            }
            catch (Exception e)
            {
                _logger.Error("Could not execute task " + GetType().Name, e);
                OnError(e);
                return false;
            }
            finally
            {
                TaskExecutor.Discard();
            }
        }

        public abstract void Execute();
    }
}
