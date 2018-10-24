using FluentAssertions;
using System;
using Xunit;

namespace BallBusterX
{
    public class BallTests
    {
        [Fact]
        public void SpikeCount()
        {
            var b = new Ball();

            b.Power = 1;
            b.Spikes.Should().Be(3);

            b.Power = 2;
            b.Spikes.Should().Be(4);

            b.Power = 3;
            b.Spikes.Should().Be(5);

            b.Power = 4;
            b.Spikes.Should().Be(6);

            b.Power = 5;
            b.Power.Should().Be(4);
        }
    }
}
