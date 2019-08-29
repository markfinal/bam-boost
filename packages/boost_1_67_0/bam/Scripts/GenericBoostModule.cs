#region License
// Copyright (c) 2010-2019, Mark Final
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
    interface IConfigureBoost :
        Bam.Core.IModuleConfiguration
    {
        bool EnableAutoLinking { get; }
    }

    sealed class ConfigureBoost :
        IConfigureBoost
    {
        public ConfigureBoost(
            Bam.Core.Environment buildEnvironment) => this.EnableAutoLinking = true;

        public bool EnableAutoLinking { get; set; }
    }

    [Bam.Core.ModuleGroup("Thirdparty/Boost")]
    abstract class GenericBoostModule :
        C.Cxx.DynamicLibrary,
        Bam.Core.IHasModuleConfiguration
    {
        global::System.Type Bam.Core.IHasModuleConfiguration.ReadOnlyInterfaceType => typeof(IConfigureBoost);
        global::System.Type Bam.Core.IHasModuleConfiguration.WriteableClassType => typeof(ConfigureBoost);

        protected GenericBoostModule(
            string name) => this.Name = name;

        private string Name { get; set; }
        protected C.Cxx.ObjectFileCollection BoostSource { get; private set; }
        protected C.HeaderFileCollection BoostHeaders { get; private set; }

        protected override void
        Init()
        {
            base.Init();

            this.SetSemanticVersion(1, 6, 7);

            if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Windows))
            {
                var visualC = Bam.Core.Graph.Instance.Packages.First(item => item.Name == "VisualC");
                string vcVer = string.Empty;
                if (visualC.Version == "16")
                {
                    vcVer = "142";
                }
                else if (visualC.Version == "15.0")
                {
                    vcVer = "141";
                }
                else if (visualC.Version == "14.0")
                {
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
                else if (visualC.Version == "10.0")
                {
                    vcVer = "100";
                }
                else
                {
                    throw new Bam.Core.Exception($"Unsupported version of VisualC, {visualC.Version}");
                }
                this.Macros[Bam.Core.ModuleMacroNames.OutputName] = this.CreateTokenizedString(
                    string.Format("boost_{0}-vc{1}-$(boost_vc_mode)-{2}_{3}{4}",
                                  this.Name,
                                  vcVer,
                                  this.Macros[C.ModuleMacroNames.MajorVersion].ToString(),
                                  this.Macros[C.ModuleMacroNames.MinorVersion].ToString(),
                                  this.Macros[C.ModuleMacroNames.PatchVersion].ToString()));
            }
            else if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Linux))
            {
                this.Macros[Bam.Core.ModuleMacroNames.OutputName] = TokenizedString.CreateVerbatim(string.Format("boost_{0}", this.Name));
            }
            else if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.OSX))
            {
                this.Macros[Bam.Core.ModuleMacroNames.OutputName] = TokenizedString.CreateVerbatim(string.Format("boost_{0}", this.Name));
            }
            else
            {
                throw new Bam.Core.Exception("Invalid platform for Boost builds");
            }

            this.BoostHeaders = this.CreateHeaderCollection(string.Format("$(packagedir)/boost/{0}/**.hpp", this.Name));

            this.BoostSource = this.CreateCxxSourceCollection();
            this.BoostSource.ClosingPatch(settings =>
                {
                    if (settings is VisualCCommon.ICommonCompilerSettings vcCompiler)
                    {
                        // boost_vc_mode is a macro used on the link step, so must use the encapsulating module of the source
                        // (since it depends on a compilation property)
                        var encapsulating = this.EncapsulatingModule;
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
                    if (settings is C.ICommonPreprocessorSettings preprocessor)
                    {
                        preprocessor.SystemIncludePaths.AddUnique(this.CreateTokenizedString("$(packagedir)"));
                        var configuration = this.Configuration as IConfigureBoost;
                        if (!configuration.EnableAutoLinking)
                        {
                            preprocessor.PreprocessorDefines.Add("BOOST_ALL_NO_LIB");
                        }
                        preprocessor.PreprocessorDefines.Add("BOOST_ALL_DYN_LINK");
                    }
                });

            this.BoostSource.PrivatePatch(settings =>
                {
                    var cxxCompiler = settings as C.ICxxOnlyCompilerSettings;
                    cxxCompiler.LanguageStandard = C.Cxx.ELanguageStandard.Cxx11;
                    cxxCompiler.StandardLibrary = C.Cxx.EStandardLibrary.libcxx;
                    cxxCompiler.ExceptionHandler = C.Cxx.EExceptionHandler.Asynchronous;

                    if (settings is VisualCCommon.ICommonCompilerSettings vcCompiler)
                    {
                        vcCompiler.WarningLevel = VisualCCommon.EWarningLevel.Level4;
                    }

                    if (settings is GccCommon.ICommonCompilerSettings gccCompiler)
                    {
                        gccCompiler.AllWarnings = true;
                        gccCompiler.ExtraWarnings = true;
                        gccCompiler.Pedantic = true;
                    }

                    if (settings is ClangCommon.ICommonCompilerSettings clangCompiler)
                    {
                        clangCompiler.AllWarnings = true;
                        clangCompiler.ExtraWarnings = true;
                        clangCompiler.Pedantic = true;
                    }
                });

            this.PrivatePatch(settings =>
                {
                    if (settings is C.ICxxOnlyLinkerSettings cxxLinker)
                    {
                        cxxLinker.StandardLibrary = C.Cxx.EStandardLibrary.libcxx;
                    }
                });
        }
    }
}
