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
    [Bam.Core.ModuleGroup("Thirdparty/Boost")]
    class SDK :
        C.SDKTemplate
    {
        private readonly Bam.Core.TypeArray libraryTypes = new Bam.Core.TypeArray(
            /*
            typeof(Config),
            typeof(SmartPtr),
            typeof(Predef),
            typeof(TypeTraits),
            typeof(Mpl),
            typeof(Preprocessor),
            typeof(Numeric),
            typeof(Utility),
            typeof(Ratio),
            typeof(Intrusive),
            typeof(Move),
            typeof(Bind),
            typeof(IO),
            typeof(Functional),
            typeof(ContainerHash),
            typeof(Integer),
            typeof(Tuple),
            typeof(Iterator),
            typeof(Optional),
            typeof(Function),
            typeof(TypeIndex),
            typeof(DateTime),
            typeof(Chrono),
            typeof(System),
            typeof(Atomic),
            typeof(Container)
            */
            typeof(Thread)
        );

        public SDK()
        {
            /*
            this.headers = new Bam.Core.StringArray
            {
                "boost/winapi/**", // TODO: Windows only?
                "boost/exception_ptr.hpp",
                "boost/intrusive_ptr.hpp",
                "boost/intrusive/**",
                "boost/integer.hpp",
                "boost/integer_traits.hpp",
                "boost/scoped_array.hpp",
                "boost/optional.hpp",
                "boost/optional/**",
                "boost/none.hpp",
                "boost/none_t.hpp",
                "boost/enable_shared_from_this.hpp",
                "boost/function.hpp",
                "boost/function_equal.hpp",
                "boost/function_output_iterator.hpp",
                "boost/function/**",
                "boost/type_index.hpp",
                "boost/type_index/**",
                "boost/memory_order.hpp",
                "boost/container/**",
            };
            */
        }

        protected override Bam.Core.TypeArray LibraryModuleTypes => this.libraryTypes;
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
