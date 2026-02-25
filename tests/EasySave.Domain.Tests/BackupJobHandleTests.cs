using EasySave.Domain.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EasySave.Tests.Domain.Models
{
    public class BackupJobHandleTests
    {
        // Checks that a new handle is not paused by default
        [Fact]
        public void NewHandle_Should_NotBePaused()
        {
            var handle = new BackupJobHandle();
            Assert.False(handle.IsPaused);
        }

        // Checks that Pause sets IsPaused to true
        [Fact]
        public void Pause_Should_SetIsPausedTrue()
        {
            var handle = new BackupJobHandle();
            handle.Pause();
            Assert.True(handle.IsPaused);
        }

        // Checks that Resume sets IsPaused to false
        [Fact]
        public void Resume_Should_SetIsPausedFalse()
        {
            var handle = new BackupJobHandle();
            handle.Pause();
            handle.Resume();
            Assert.False(handle.IsPaused);
        }

        // Checks that WaitIfPaused blocks when paused and continues when resumed
        [Fact]
        public void WaitIfPaused_Should_Block_WhenPaused_And_Unblock_WhenResumed()
        {
            var handle = new BackupJobHandle();
            handle.Pause();

            bool waited = false;

            var task = Task.Run(() =>
            {
                handle.WaitIfPaused();
                waited = true;
            });

            // Give some time to ensure task is blocked
            Thread.Sleep(100);
            Assert.False(waited);

            handle.Resume();
            task.Wait(1000); // Wait for task to finish

            Assert.True(waited);
            Assert.False(handle.IsPaused);
        }

        // Checks that Stop cancels the token and unblocks WaitIfPaused
        [Fact]
        public void Stop_Should_CancelToken_And_UnblockWaitIfPaused()
        {
            var handle = new BackupJobHandle();
            handle.Pause();

            bool taskCompleted = false;

            var task = Task.Run(() =>
            {
                handle.WaitIfPaused(); // This will unblock either via Resume or Stop
                taskCompleted = true;
            });

            Thread.Sleep(100);
            Assert.False(taskCompleted);

            handle.Stop(); // Should unblock WaitIfPaused and cancel the token
            task.Wait(1000); // Wait for task to finish

            Assert.True(taskCompleted, "WaitIfPaused did not complete after Stop");
            Assert.True(handle.CancellationToken.IsCancellationRequested, "CancellationToken should be canceled after Stop");
        }

        // Checks that Dispose does not throw and frees resources
        [Fact]
        public void Dispose_Should_NotThrow()
        {
            var handle = new BackupJobHandle();
            handle.Dispose();
        }
    }
}