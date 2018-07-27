#region License
// Copyright (c) 2010-2018, Mark Final
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
//
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
//
// * Neither the name of BuildAMation nor the names of its
//   contributors may be used to endorse or promote products derived from
//   this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion // License
using Bam.Core;
using System.Linq;
namespace boost
{
#if BAM_FEATURE_MODULE_CONFIGURATION
    interface IConfigureBoost :
        Bam.Core.IModuleConfiguration
    {
        bool EnableAutoLinking
        {
            get;
        }
    }

    sealed class ConfigureBoost :
        IConfigureBoost
    {
        public ConfigureBoost(
            Bam.Core.Environment buildEnvironment)
        {
            this.EnableAutoLinking = true;
        }

        public bool EnableAutoLinking
        {
            get;
            set;
        }
    }
#endif

    [Bam.Core.ModuleGroup("Thirdparty/Boost")]
    abstract class GenericBoostModule :
        C.Cxx.DynamicLibrary
#if BAM_FEATURE_MODULE_CONFIGURATION
        , Bam.Core.IHasModuleConfiguration
#endif
    {
#if BAM_FEATURE_MODULE_CONFIGURATION
        global::System.Type IHasModuleConfiguration.ReadOnlyInterfaceType
        {
            get
            {
                return typeof(IConfigureBoost);
            }
        }

        global::System.Type IHasModuleConfiguration.WriteableClassType
        {
            get
            {
                return typeof(ConfigureBoost);
            }
        }
#endif

        protected GenericBoostModule(
            string name)
        {
            this.Name = name;
        }

        private string Name
        {
            get;
            set;
        }

        protected C.Cxx.ObjectFileCollection BoostSource
        {
            get;
            private set;
        }

        protected C.HeaderFileCollection BoostHeaders
        {
            get;
            private set;
        }

        protected override void
        Init(
            Bam.Core.Module parent)
        {
            base.Init(parent);

            this.SetSemanticVersion(1, 6, 7);

            if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Windows))
            {
                var visualC = Bam.Core.Graph.Instance.Packages.First(item => item.Name == "VisualC");
                string vcVer = string.Empty;
                if (visualC.Version == "14.0" ||
                    visualC.Version == "15.0")
                {
                    // VS 2017 is backward compatible with VS2015
                    vcVer = "140";
                }
                else if (visualC.Version == "12.0")
                {
                    vcVer = "120";
                }
                else if (visualC.Version == "11.0")
                {
                    vcVer = "110";
                }
                else
                {
                    throw new Bam.Core.Exception("Unsupported version of VisualC, {0}", visualC.Version);
                }
                this.Macros["OutputName"] = this.CreateTokenizedString(
                    string.Format("boost_{0}-vc{1}-$(boost_vc_mode)-{2}_{3}{4}",
                                  this.Name,
                                  vcVer,
                                  this.Macros["MajorVersion"].ToString(),
                                  this.Macros["MinorVersion"].ToString(),
                                  this.Macros["PatchVersion"].ToString()));
            }
            else if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Linux))
            {
                this.Macros["OutputName"] = TokenizedString.CreateVerbatim(string.Format("boost_{0}", this.Name));
            }
            else if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.OSX))
            {
                this.Macros["OutputName"] = TokenizedString.CreateVerbatim(string.Format("boost_{0}", this.Name));
            }
            else
            {
                throw new Bam.Core.Exception("Invalid platform for Boost builds");
            }

            this.BoostHeaders = this.CreateHeaderContainer(string.Format("$(packagedir)/boost/{0}/**.hpp", this.Name));

            this.BoostSource = this.CreateCxxSourceContainer();
            this.BoostSource.ClosingPatch(settings =>
                {
                    var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                    if (null != vcCompiler)
                    {
                        // boost_vc_mode is a macro used on the link step, so must use the encapsulating module of the source
                        // (since it depends on a compilation property)
                        var encapsulating = this.GetEncapsulatingReferencedModule();
                        if (vcCompiler.RuntimeLibrary == VisualCCommon.ERuntimeLibrary.MultiThreadedDebugDLL)
                        {
                            encapsulating.Macros["boost_vc_mode"] = TokenizedString.CreateVerbatim("mt-gd");
                        }
                        else
                        {
                            encapsulating.Macros["boost_vc_mode"] = TokenizedString.CreateVerbatim("mt");
                        }
                    }
                });

            this.PublicPatch((settings, appliedTo) =>
                {
                    var compiler = settings as C.ICommonCompilerSettings;
                    if (null != compiler)
                    {
                        compiler.IncludePaths.AddUnique(this.CreateTokenizedString("$(packagedir)"));
#if BAM_FEATURE_MODULE_CONFIGURATION
                        var configuration = this.Configuration as IConfigureBoost;
                        if (!configuration.EnableAutoLinking)
                        {
                            compiler.PreprocessorDefines.Add("BOOST_ALL_NO_LIB");
                        }
#endif
                        compiler.PreprocessorDefines.Add("BOOST_ALL_DYN_LINK");
                    }
                });

            this.BoostSource.PrivatePatch(settings =>
                {
                    var cxxCompiler = settings as C.ICxxOnlyCompilerSettings;
                    cxxCompiler.ExceptionHandler = C.Cxx.EExceptionHandler.Asynchronous;

                    var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                    if (null != vcCompiler)
                    {
                        vcCompiler.WarningLevel = VisualCCommon.EWarningLevel.Level4;
                    }

                    var gccCompiler = settings as GccCommon.ICommonCompilerSettings;
                    if (null != gccCompiler)
                    {
                        gccCompiler.AllWarnings = true;
                        gccCompiler.ExtraWarnings = true;
                        gccCompiler.Pedantic = true;
                    }

                    var clangCompiler = settings as ClangCommon.ICommonCompilerSettings;
                    if (null != clangCompiler)
                    {
                        clangCompiler.AllWarnings = true;
                        clangCompiler.ExtraWarnings = true;
                        clangCompiler.Pedantic = true;
                    }
                });
        }
    }
}
