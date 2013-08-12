using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Raven.Client;

namespace MAT.Core.Tasks
{
    public class TaskExecutor
    {
        private static readonly ThreadLocal<List<BackgroundTask>> tasksToExecute = new ThreadLocal<List<BackgroundTask>>(() => new List<BackgroundTask>());
        public static Action<Exception> ExceptionHandler { get; set; }

        private static IDocumentStore _documentStore;
        public static IDocumentStore DocumentStore
        {
            get { return _documentStore; }
            set
            {
                if (_documentStore == null)
                {
                    _documentStore = value;
                }
            }
        }

        public static void ExecuteLater(BackgroundTask task)
        {
            tasksToExecute.Value.Add(task);
        }

        public static void Discard()
        {
            tasksToExecute.Value.Clear();
        }

        public static void StartExecuting()
        {
            var value = tasksToExecute.Value;
            var copy = value.ToArray();
            value.Clear();

            if (copy.Length > 0)
            {
                Task.Factory.StartNew(() =>
                    {
                        foreach (var backgroundTask in copy)
                        {
                            ExecuteTask(backgroundTask);
                            if (backgroundTask.RecurringTask)
                            {
                                ExecuteLater(backgroundTask);
                            }
                        }
                    }, TaskCreationOptions.LongRunning)
                    .ContinueWith(task => { if (ExceptionHandler != null) ExceptionHandler(task.Exception); }, TaskContinuationOptions.OnlyOnFaulted);
            }
        }

        public static void ExecuteTask(BackgroundTask task)
        {
            for (var i = 0; i < 10; i++)
            {
                using (var session = _documentStore.OpenSession())
                {
                    switch (task.Run(session))
                    {
                        case true:
                        case false:
                            return;
                        case null:
                            break;
                    }
                }
            }
        }
    }
}
