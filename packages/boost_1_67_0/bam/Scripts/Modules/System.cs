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
namespace boost
{
    class System :
        GenericBoostModule,
        C.IPublicHeaders
    {
        public System() :
            base("system")
        {}

        Bam.Core.StringArray C.IPublicHeaders.PublicHeaders { get; } = new Bam.Core.StringArray(
            "boost/system/**",
            "boost/cerrno.hpp"
        );

        protected override void
        Init()
        {
            base.Init();

            this.BoostSource.AddFiles("$(packagedir)/libs/system/src/*.cpp");

            this.BoostHeaders.AddFile("$(packagedir)/boost/cerrno.hpp");

            this.CompileAgainstPublicly<Config>(this.BoostSource);
            this.CompileAgainstPublicly<Predef>(this.BoostSource);
            this.CompileAgainstPublicly<Integer>(this.BoostSource);
            this.CompileAgainstPublicly<Assert>(this.BoostSource);
            this.CompileAgainstPublicly<Core>(this.BoostSource);
            this.CompileAgainstPublicly<Utility>(this.BoostSource);

            /*
            this.PublicPatch((settings, appliedTo) =>
                {
                    if (settings is GccCommon.ICommonCompilerSettings)
                    {
                        var compiler = settings as C.ICommonCompilerSettings;
                        compiler.DisableWarnings.AddUnique("unused-variable"); // boost_1_60_0/boost/system/error_code.hpp:221:36: error: 'boost::system::posix_category' defined but not used
                    }
                });
                */
        }
    }

    namespace tests
    {
        class ErrorCodeTest :
            GenericBoostTest
        {
            protected override void
            Init()
            {
                base.Init();

                this.TestSource.AddFiles("$(packagedir)/libs/system/test/error_code_test.cpp");
                /*
                this.CompileAndLinkAgainst<System>(this.TestSource);
                */
            }
        }

        class ErrorCodeUserTest :
            GenericBoostTest
        {
            protected override void
            Init()
            {
                base.Init();

                this.TestSource.AddFiles("$(packagedir)/libs/system/test/error_code_user_test.cpp");
                /*
                this.CompileAndLinkAgainst<System>(this.TestSource);
                */
            }
        }

        class SystemErrorTest :
            GenericBoostTest
        {
            protected override void
            Init()
            {
                base.Init();

                this.TestSource.AddFiles("$(packagedir)/libs/system/test/system_error_test.cpp");
                /*
                this.CompileAndLinkAgainst<System>(this.TestSource);
                */
            }
        }

        class DynamicLinkTest :
            GenericBoostTest
        {
            protected override void
            Init()
            {
                base.Init();

                this.TestSource.AddFiles("$(packagedir)/libs/system/test/dynamic_link_test.cpp");
                this.TestSource.AddFiles("$(packagedir)/libs/system/test/throw_test.cpp");
                /*
                this.CompileAndLinkAgainst<System>(this.TestSource);
                */
            }
        }

        class InitializationTest :
            GenericBoostTest
        {
            protected override void
            Init()
            {
                base.Init();

                this.TestSource.AddFiles("$(packagedir)/libs/system/test/initialization_test.cpp");
                /*
                this.CompileAndLinkAgainst<System>(this.TestSource);
                */
            }
        }

        class HeaderOnlyTest :
            GenericBoostTest
        {
            protected override void
            Init()
            {
                base.Init();

                this.TestSource.AddFiles("$(packagedir)/libs/system/test/header_only_test.cpp");
                /*
                this.CompileAndLinkAgainst<System>(this.TestSource);
                */
            }
        }

        class ConfigTest :
            GenericBoostTest
        {
            protected override void
            Init()
            {
                base.Init();

                this.TestSource.AddFiles("$(packagedir)/libs/system/test/config_test.cpp");
                /*
                this.CompileAndLinkAgainst<System>(this.TestSource);
                */
            }
        }
    }
}
