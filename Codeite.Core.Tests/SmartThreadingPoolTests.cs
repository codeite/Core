using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Codeite.Core.Threading;
using NUnit.Framework;
using Shouldly;

namespace Codeite.Core.Tests
{
    [TestFixture, Timeout(200000)]
    class SmartThreadingPoolTests
    {
        [SetUp]
        public void SetUp()
        {
            Thread.CurrentThread.Name = "Master";
        }

        [Test]
        public void ShouldTerminateNormallyWhenInstructed()
        {
            // Arrange
            var smartThreadPool = new SmartThreadPool(1);
            var executed = false;
            Action noop = () => { executed = true; };

            // Act
            smartThreadPool.Queue(noop);
            int failureCount = smartThreadPool.Terminate(TimeSpan.FromSeconds(100));

            // Assert
            executed.ShouldBe(true);
            failureCount.ShouldBe(0);
        }

        [Test]
        public void ShouldTerminateUnresponsiceThredsWhenInstructed()
        {
            // Arrange
            var smartThreadPool = new SmartThreadPool(1);
            var executed = false;

            // Act
            smartThreadPool.Queue(() =>
            {
                executed = true;
                Thread.Sleep(TimeSpan.FromSeconds(10));
            });
            Thread.Sleep(50);
            var failureCount = smartThreadPool.Terminate(TimeSpan.FromSeconds(0.1));

            // Assert
            executed.ShouldBe(true);
            failureCount.ShouldBe(1);
        }

        [Test]
        public void ShouldNotExcuteActionsUntilUnPaused()
        {
            // Arrange
            var smartThreadPool = new SmartThreadPool(1, true);
            var eventLog = new List<string>();

            // Act
            eventLog.Add("Pre queue event");
            smartThreadPool.Queue(() => eventLog.Add("Queued event"));
            Thread.Sleep(50);
            eventLog.Add("Post queue event");
            smartThreadPool.UnPause();
            smartThreadPool.TerminateWhenComplete();
            eventLog.Add("Final event");

            // Assert
            eventLog.ShouldBe(new List<string>()
            {
                "Pre queue event",
                "Post queue event",
                "Queued event",
                "Final event"
            });
        }

        [Test]
        public void ShouldExcuteActionsImidiatly()
        {
            // Arrange
            var smartThreadPool = new SmartThreadPool(1);
            var eventLog = new List<string>();

            // Act
            eventLog.Add("Pre queue event");
            smartThreadPool.Queue(() => eventLog.Add("Queued event"));
            Thread.Sleep(50);
            eventLog.Add("Post queue event");
            smartThreadPool.UnPause();
            smartThreadPool.TerminateWhenComplete();
            eventLog.Add("Final event");

            // Assert
            eventLog.ShouldBe(new List<string>()
            {
                "Pre queue event",
                "Queued event",
                "Post queue event",
                "Final event"
            });
        }

        [Test]
        public void ShouldSpawnCorrectNumberOfWorkers()
        {
            // Arrange
            var smartThreadPool = new SmartThreadPool(10, true);
            var threadCount = 0;
            var manualResertEventSlim = new ManualResetEventSlim();

            // Act
            for (var i = 0; i < 20; i++)
            {
                smartThreadPool.Queue(() =>
                {
                    Interlocked.Increment(ref threadCount);
                    manualResertEventSlim.Wait();
                });
            }
            smartThreadPool.UnPause();
            smartThreadPool.ClearBacklok();
            manualResertEventSlim.Set();
            smartThreadPool.TerminateWhenComplete();

            // Assert
            Console.WriteLine(threadCount);
            threadCount.ShouldBe(10);
        }
    }
}
