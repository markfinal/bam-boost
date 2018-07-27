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
namespace boost
{
    class Chrono :
        GenericBoostModule
    {
        public Chrono() :
            base("chrono")
        {}

        protected override void
        Init(
            Module parent)
        {
            base.Init(parent);

            this.BoostSource.AddFiles("$(packagedir)/libs/chrono/src/*.cpp");

            if (this is C.Cxx.DynamicLibrary)
            {
                this.LinkAgainst<System>();

                this.BoostSource.PrivatePatch(settings =>
                    {
                        if (this.BuildEnvironment.Configuration != Bam.Core.EConfiguration.Debug)
                        {
                            if (settings is GccCommon.ICommonCompilerSettings)
                            {
                                if (this.BoostSource.Compiler.IsAtLeast(5, 4))
                                {
                                    var compiler = settings as C.ICommonCompilerSettings;
                                    compiler.DisableWarnings.AddUnique("unused-variable"); // boost_1_60_0/boost/system/error_code.hpp:221:36: error: 'boost::system::posix_category' defined but not used [-Werror=unused-variable]
                                }
                            }
                        }
                    });
            }
        }
    }

    namespace tests
    {
        class IntMax :
            GenericBoostTest
        {
            protected override void
            Init(
                Bam.Core.Module parent)
            {
                base.Init(parent);

                this.TestSource.AddFiles("$(packagedir)/libs/chrono/test/intmax_c.cpp");
                this.CompileAndLinkAgainst<Chrono>(this.TestSource);
            }
        }

        class OneObj :
            GenericBoostTest
        {
            protected override void
            Init(
                Bam.Core.Module parent)
            {
                base.Init(parent);

                this.TestSource.AddFiles("$(packagedir)/libs/chrono/test/one_obj.cpp");
                this.TestSource.AddFiles("$(packagedir)/libs/chrono/test/another_obj.cpp");
                this.CompileAndLinkAgainst<Chrono>(this.TestSource);
                this.CompileAndLinkAgainst<System>(this.TestSource);
            }
        }

        class Clock :
            GenericBoostTest
        {
            protected override void
            Init(
                Bam.Core.Module parent)
            {
                base.Init(parent);

                this.TestSource.AddFiles("$(packagedir)/libs/chrono/test/clock/*.cpp");
                this.CompileAndLinkAgainst<Chrono>(this.TestSource);
                this.CompileAndLinkAgainst<System>(this.TestSource);
            }
        }

        class DurationArithmeticPass :
            GenericBoostTest
        {
            protected override void
            Init(
                Bam.Core.Module parent)
            {
                base.Init(parent);

                this.TestSource.AddFiles("$(packagedir)/libs/chrono/test/duration/arithmetic_pass.cpp");
                this.CompileAndLinkAgainst<Chrono>(this.TestSource);
            }
        }

        class DurationComparisonsPass :
            GenericBoostTest
        {
            protected override void
            Init(
                Bam.Core.Module parent)
            {
                base.Init(parent);

                this.TestSource.AddFiles("$(packagedir)/libs/chrono/test/duration/comparisons_pass.cpp");
                this.CompileAndLinkAgainst<Chrono>(this.TestSource);
            }
        }

        class DurationConstructorPass :
            GenericBoostTest
        {
            protected override void
            Init(
                Bam.Core.Module parent)
            {
                base.Init(parent);

                this.TestSource.AddFiles("$(packagedir)/libs/chrono/test/duration/constructor_pass.cpp");
                this.CompileAndLinkAgainst<Chrono>(this.TestSource);
            }
        }

        class DurationCastPass :
            GenericBoostTest
        {
            protected override void
            Init(
                Bam.Core.Module parent)
            {
                base.Init(parent);

                this.TestSource.AddFiles("$(packagedir)/libs/chrono/test/duration/duration_cast_pass.cpp");
                this.CompileAndLinkAgainst<Chrono>(this.TestSource);
            }
        }

        class DurationValuesPass :
            GenericBoostTest
        {
            protected override void
            Init(
                Bam.Core.Module parent)
            {
                base.Init(parent);

                this.TestSource.AddFiles("$(packagedir)/libs/chrono/test/duration/duration_values_pass.cpp");
                this.CompileAndLinkAgainst<Chrono>(this.TestSource);
            }
        }

        class DurationRoundingPass :
            GenericBoostTest
        {
            protected override void
            Init(
                Bam.Core.Module parent)
            {
                base.Init(parent);

                this.TestSource.AddFiles("$(packagedir)/libs/chrono/test/duration/rounding_pass.cpp");
                this.CompileAndLinkAgainst<Chrono>(this.TestSource);
                this.CompileAndLinkAgainst<System>(this.TestSource);
            }
        }

        class IODurationInput :
            GenericBoostTest
        {
            protected override void
            Init(
                Bam.Core.Module parent)
            {
                base.Init(parent);

                this.TestSource.AddFiles("$(packagedir)/libs/chrono/test/io/duration_input.cpp");
                this.CompileAndLinkAgainst<Chrono>(this.TestSource);
                this.CompileAndLinkAgainst<System>(this.TestSource);
            }
        }

        class IODurationOutput :
            GenericBoostTest
        {
            protected override void
            Init(
                Bam.Core.Module parent)
            {
                base.Init(parent);

                this.TestSource.AddFiles("$(packagedir)/libs/chrono/test/io/duration_output.cpp");
                this.CompileAndLinkAgainst<Chrono>(this.TestSource);
                this.CompileAndLinkAgainst<System>(this.TestSource);
            }
        }

        class IOTimePointInput :
            GenericBoostTest
        {
            protected override void
            Init(
                Bam.Core.Module parent)
            {
                base.Init(parent);

                this.TestSource.AddFiles("$(packagedir)/libs/chrono/test/io/time_point_input.cpp");
                this.CompileAndLinkAgainst<Chrono>(this.TestSource);
                this.CompileAndLinkAgainst<System>(this.TestSource);
            }
        }

        class IOTimePointOutput :
            GenericBoostTest
        {
            protected override void
            Init(
                Bam.Core.Module parent)
            {
                base.Init(parent);

                this.TestSource.AddFiles("$(packagedir)/libs/chrono/test/io/time_point_output.cpp");
                this.CompileAndLinkAgainst<Chrono>(this.TestSource);
                this.CompileAndLinkAgainst<System>(this.TestSource);
            }
        }
    }
}
