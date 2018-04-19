
using System;

namespace ESS.FW.Common.Utilities
{
	/// <summary>
	///		Exposes version information for this assembly.
	/// </summary>
	public class VersionInfo 
	{
        private String assemblyNameValue = String.Empty;
        private String assemblyVersionNumberValue = String.Empty;
        private String assemblyFileNameValue = String.Empty;
        private String assemblyFileNameFullyQualifiedValue = String.Empty;

		private static volatile VersionInfo instanceValue;
		private static object syncRoot = new Object();

        private VersionInfo()
        {
        }

		/// <summary>
		///     Property that must be used to get visibility to the
		///     Singleton instance of this class.
		/// </summary>
		/// <returns>
		///     
		/// </returns>
		public static VersionInfo Instance
		{

			get 
			{
				if (instanceValue == null)
				{
					lock (syncRoot) 
					{
						if (instanceValue == null)
						{
							instanceValue = new VersionInfo();
							instanceValue.LoadVersionInfo();
						}
					}
				}
				return instanceValue;
			}

		}

        /// <summary>
        ///     Returns the fully qualified name of the file that contains the assembly. 
        ///     The returned value will look something like: c:\program files\\.Rebar.dll
        /// </summary>
        /// <returns>
        ///     String
        /// </returns>
        public String AssemblyFileNameFullyQualified
        {
            get
            {
                return this.assemblyFileNameFullyQualifiedValue;
            }
        }

        /// <summary>
        ///     Returns the name of the assembly.  Something like: .Rebar
        /// </summary>
        /// <returns>
        ///     string
        /// </returns>
        public String AssemblyName
        {
            get
            {
                return this.assemblyNameValue;
            }
        }


        /// <summary>
        ///     Returns the version number of the assembly.  The 
        ///     returned value will look something like: 1.0.0.4
        /// </summary>
        /// <returns>
        ///     String
        /// </returns>
        public String AssemblyVersionNumber
        {
            get
            {
                return this.assemblyVersionNumberValue;
            }
        }

        /// <summary>
        ///     Returns the name of the file that contains the assembly.  The 
        ///     returned value will look something like: .Rebar.dll
        /// </summary>
        /// <returns>
        ///     String
        /// </returns>
        public String AssemblyFileName
        {
            get
            {
                return this.assemblyFileNameValue;
            }
        }

        /// <summary>
        ///     Loads all of the property values of this class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031")]
        protected void LoadVersionInfo()
        {

            try
            {
                System.Reflection.Assembly myAssembly =
                    System.Reflection.Assembly.GetAssembly(this.GetType());
                System.Diagnostics.FileVersionInfo versionInfo =
                    System.Diagnostics.FileVersionInfo.GetVersionInfo(myAssembly.Location);
                this.assemblyFileNameFullyQualifiedValue = myAssembly.Location;
                this.assemblyVersionNumberValue = versionInfo.FileVersion;
                this.assemblyFileNameValue = versionInfo.InternalName;
                this.assemblyNameValue = myAssembly.GetName().Name;
            }
            catch (System.Exception)
            {
                this.assemblyFileNameFullyQualifiedValue = "UNKNOWN";
                this.assemblyVersionNumberValue = "UNKNOWN";
                this.assemblyFileNameValue = "UNKNOWN";
                this.assemblyNameValue = "UNKNOWN";
            }

        }
	}
}
