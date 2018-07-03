﻿using System;
using System.Collections.Generic;
using System.Text;
using MathNet.Numerics.Distributions;

namespace Master40.Tools.Simulation
{
    public class TimeGenerator
    {
        public TimeGenerator(int seed, double deviation, int simNumber)
        {
            var source = new Random(seed+simNumber);
            _distribution = new LogNormal(0, deviation, source);
        }

        private readonly LogNormal _distribution;
        /// <summary>
        /// Returns LogNormal-distributed worktime.
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public int GetRandomWorkTime(int duration)
        {
            int newDuration;
            while (true)
            {
                newDuration = (int)Math.Round(duration * _distribution.Sample(), MidpointRounding.AwayFromZero);
                if (newDuration <= 3 * duration) break;
            }
            if (duration != newDuration)
            {
                var a = 1;
            }
            return newDuration > 0 ? newDuration : 0;
        }
    }
}
