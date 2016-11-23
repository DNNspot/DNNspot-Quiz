using System;
using System.Reflection;

namespace DNNspot.Quiz
{
    public sealed class DnnVersionSingleton
    {
        private readonly Version dnnVersion = GetDnnVersion();
        private static DnnVersionSingleton instance = null;
        private readonly bool isDnn5 = false;
        private static readonly object padlock = new object();

        private DnnVersionSingleton()
        {
            if (this.dnnVersion.Major == 5)
            {
                this.isDnn5 = true;
            }
        }

        private static Version GetDnnVersion()
        {
            Version version = Assembly.GetAssembly(typeof(DotNetNuke.Common.Globals)).GetName().Version;
            if (version != null)
            {
                return version;
            }
            return null;
        }

        public Version DnnVersion
        {
            get
            {
                return this.dnnVersion;
            }
        }

        public static DnnVersionSingleton Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new DnnVersionSingleton();
                    }
                    return instance;
                }
            }
        }

        public bool IsDnn5
        {
            get
            {
                return this.isDnn5;
            }
        }
    }



}
