using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DocSharp {
    /// <summary>
    /// Progress reporter and job handler.
    /// </summary>
    public class TaskEngine {
        readonly static TimeSpan lazyStatusDelta = new TimeSpan(0, 0, 1);

        Task operation;
        ToolStripProgressBar progressBar;
        ToolStripStatusLabel progressLabel;
        DateTime lastLazyStatus = DateTime.MinValue;

        /// <summary>
        /// A task is running and is not completed or failed.
        /// </summary>
        public bool IsOperationRunning => operation != null && operation.Status == TaskStatus.Running;

        /// <summary>
        /// Current value of the progress bar.
        /// </summary>
        public int Progress {
            get {
                int val = -1;
                if (progressBar != null)
                    progressBar.GetCurrentParent().Invoke((MethodInvoker)delegate { val = progressBar.Value; });
                return val;
            }
        }

        /// <summary>
        /// Set the progress bar and status label to enable progress reporting on the UI.
        /// </summary>
        public void SetProgressReporting(ToolStripProgressBar progressBar, ToolStripStatusLabel progressLabel) {
            this.progressBar = progressBar;
            this.progressLabel = progressLabel;
        }

        /// <summary>
        /// Set the progress on the progress bar if it's set.
        /// </summary>
        public void UpdateProgressBar(int progress) {
            if (progressBar != null)
                progressBar.GetCurrentParent().Invoke((MethodInvoker)delegate { progressBar.Value = progress; });
        }

        /// <summary>
        /// Set the status text label, if it's given.
        /// </summary>
        public void UpdateStatus(string text) {
            if (progressLabel != null) {
                progressLabel.GetCurrentParent().Invoke((MethodInvoker)delegate { progressLabel.Text = text; });
                lastLazyStatus = DateTime.Now;
            }
        }

        /// <summary>
        /// Set the status text label, if it's given. The label is only updated if the last update was <see cref="lazyStatusDelta"/> ago.
        /// </summary>
        public void UpdateStatusLazy(string text) {
            DateTime now = DateTime.Now;
            if (now - lastLazyStatus > lazyStatusDelta && progressLabel != null) {
                progressLabel.GetCurrentParent().Invoke((MethodInvoker)delegate { progressLabel.Text = text; });
                lastLazyStatus = now;
            }
        }

        /// <summary>
        /// Run a new task if no task is running.
        /// </summary>
        public bool Run(Action task) {
            if (IsOperationRunning) {
                MessageBox.Show("Another operation is already running.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            operation = new Task(task);
            operation.Start();
            return true;
        }
    }
}