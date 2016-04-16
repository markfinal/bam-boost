#region License
// Copyright (c) 2010-2016, Mark Final
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
    abstract class GenericBoostModule :
        C.Cxx.DynamicLibrary
    {
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

            if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Windows))
            {
                var visualC = Bam.Core.Graph.Instance.Packages.Where(item => item.Name == "VisualC").First();
                string vcVer = string.Empty;
                if (visualC.Version == "14.0")
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
                else
                {
                    throw new Bam.Core.Exception("Unsupported version of VisualC, {0}", visualC.Version);
                }
                this.Macros["OutputName"] = this.CreateTokenizedString(string.Format("boost_{0}-vc{1}-$(boost_vc_mode)-1_60", this.Name, vcVer));
            }
            else if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.Linux))
            {
                // TODO: validate
                this.Macros["OutputName"] = TokenizedString.CreateVerbatim(string.Format("boost_{0}-1_60", this.Name));
            }
            else if (this.BuildEnvironment.Platform.Includes(Bam.Core.EPlatform.OSX))
            {
                // TODO: validate
                this.Macros["OutputName"] = TokenizedString.CreateVerbatim(string.Format("boost_{0}-1_60", this.Name));
            }
            else
            {
                throw new Bam.Core.Exception("Invalid platform for Boost builds");
            }

            this.BoostHeaders = this.CreateHeaderContainer(string.Format("$(packagedir)/boost/{0}/**.hpp", this.Name));

            this.BoostSource = this.CreateCxxSourceContainer();
            this.BoostSource.PrivatePatch(settings =>
                {
                    var vcCompiler = settings as VisualCCommon.ICommonCompilerSettings;
                    if (null != vcCompiler)
                    {
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
                });

            if (this.Linker is VisualCCommon.LinkerBase)
            {
                this.CompilePubliclyAndLinkAgainst<WindowsSDK.WindowsSDK>(this.BoostSource);
            }
        }
    }
}
