using System.Globalization;
using System.Threading;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace PressPlay.FFWD.BuildTasks
{
    public class SetLocale : Task
    {
        private string locale = "en-US";

        /// <summary>
        /// Gets or sets the locale to establish. By default, en-US.
        /// </summary>
        /// <value>
        /// The locale to set.
        /// </value>
        public string Locale
        {
            get { return this.locale; }
            set { this.locale = value; }
        }

        [Output]
        public string PrevLocale { get; set; }

        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        /// <returns>
        /// true if the task successfully executed; otherwise, false.
        /// </returns>
        public override bool Execute ()
        {
            this.PrevLocale = Thread.CurrentThread.CurrentCulture.Name;
            Thread.CurrentThread.CurrentCulture = new CultureInfo (this.Locale);

            return true;
        }
    }
}
