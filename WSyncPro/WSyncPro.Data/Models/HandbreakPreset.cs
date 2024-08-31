using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSyncPro.Data.Models
{
    public enum HandbreakPreset
    {
        // General presets
        VeryFast720p30,
        Fast720p30,
        HQ720p30Surround,
        VeryFast1080p30,
        Fast1080p30,
        Hq1080p30Surround,
        VeryFast480p30,
        Fast480p30,

        // Device-specific presets
        Apple1080p60Surround,
        AppleTV1080p30Surround,
        Roku720p30Surround,
        Roku1080p30Surround,
        FireTV720p30Surround,
        FireTV1080p30Surround,
        PlayStation42080p30,
        PlayStation320p30,
        Xbox1080p30Surround,

        // Web presets
        Web1080p30,
        Web720p30,
        Web480p30,

        // Matroska presets
        MatroskaHq1080p30Surround,
        MatroskaHq720p30Surround,
        MatroskaHq480p30Surround,
        MatroskaFast1080p30,
        MatroskaFast720p30,
        MatroskaFast480p30,

        // YouTube presets
        YouTube1080p30,
        YouTube720p30,
        YouTube480p30,

        // Custom presets
        CustomPreset1,
        CustomPreset2,
        CustomPreset3,

        // Legacy/Compatibility presets
        LegacyAppleTV,
        LegacyiPodLegacy,
        LegacyiPhoneLegacy,
        LegacyPSP
    }

}
