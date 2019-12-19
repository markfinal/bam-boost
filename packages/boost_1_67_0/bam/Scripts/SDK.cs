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
using System.Linq;
namespace boost
{
    interface IConfigureSDK :
        Bam.Core.IModuleConfiguration
    {
        Bam.Core.TypeArray IncludedModules { get; }
    }

    sealed class ConfigureSDK :
        IConfigureSDK
    {
        public ConfigureSDK(
            Bam.Core.Environment buildEnvironment)
        {
            this.IncludedModules = new Bam.Core.TypeArray(
                typeof(Thread)
            );
        }

        public Bam.Core.TypeArray IncludedModules { get; set; }
    }

    [Bam.Core.ModuleGroup("Thirdparty/Boost")]
    class SDK :
        C.SDKTemplate,
        Bam.Core.IHasModuleConfiguration
    {
        global::System.Type Bam.Core.IHasModuleConfiguration.ReadOnlyInterfaceType => typeof(IConfigureSDK);
        global::System.Type Bam.Core.IHasModuleConfiguration.WriteableClassType => typeof(ConfigureSDK);

        protected override Bam.Core.TypeArray LibraryModuleTypes { get; } = new Bam.Core.TypeArray();

        protected override Bam.Core.StringArray ExtraHeaderFiles { get; } = new Bam.Core.StringArray(
            "boost/version.hpp",
            "boost/limits.hpp",
            "boost/operators.hpp",
            "boost/type.hpp",
            "boost/get_pointer.hpp",
            "boost/is_placeholder.hpp",
            "boost/visit_each.hpp",
            "boost/memory_order.hpp"
        );

        protected override void
        Init()
        {
            this.LibraryModuleTypes.AddRange((this.Configuration as IConfigureSDK).IncludedModules);

            base.Init();

            this.PublicPatch((settings, appliedTo) =>
            {
                if (settings is C.ICommonPreprocessorSettings preprocessor)
                {
                    var configuration = this.realLibraryModules.First(item => item is GenericBoostModule).Configuration as IConfigureBoost;
                    if (!configuration.EnableAutoLinking)
                    {
                        preprocessor.PreprocessorDefines.Add("BOOST_ALL_NO_LIB");
                    }
                    preprocessor.PreprocessorDefines.Add("BOOST_ALL_DYN_LINK");
                }
            });
        }
    }
}
