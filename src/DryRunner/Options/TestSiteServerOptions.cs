using DryRunner.Exceptions;
using JetBrains.Annotations;

namespace DryRunner.Options
{
    /// <summary>
    /// Options for customizing the way a test site is hosted (inside IISExpress)
    /// </summary>
    [PublicAPI]
    public class TestSiteServerOptions
    {
        /// <summary>
        /// Port to use for the IIS Express instance. Defaults to 8888 for HTTP and 4433 for HTTPS.
        /// </summary>
        public uint? Port { get; set; }

        /// <summary>
        /// Website application path. Defaults to the server root <c>"/"</c>.
        /// </summary>
        public string ApplicationPath { get; set; }

        /// <summary>
        /// True to make the IIS Express command-line window visible, otherwise false.
        /// Defaults to true.
        /// </summary>
        public bool ShowIisExpressWindow { get; set; }

        /// <summary>
        /// Enables Windows Authentication for IIS Express.
        /// </summary>
        public bool EnableWindowsAuthentication { get; set; }

        /// <summary>
        /// Use HTTPS for hosting, default is <see langword="false" />.
        /// </summary>
        public bool UseHttps { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TestSiteServerOptions()
        {
            ApplicationPath = "/";
            ShowIisExpressWindow = true;
            Validate();
        }

        internal void FinalizeAndValidate()
        {
            ApplyDefaultsWhereNecessary();
            Validate();
        }

        private void ApplyDefaultsWhereNecessary()
        {
            if(Port == null)
              Port = (uint) (UseHttps ? 44333 : 8888);
        }

        private void Validate()
        {
            const string optionsName = "Server";

            if (ApplicationPath == null || !ApplicationPath.StartsWith("/"))
                throw new OptionValidationException("Application path must start with '/'.", optionsName, "ApplicationPath");
        }
    }
}