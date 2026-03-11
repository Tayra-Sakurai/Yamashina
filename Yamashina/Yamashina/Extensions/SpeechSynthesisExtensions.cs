// SPDX-FileCopyrightText: 2026 Tayra Sakurai
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace Yamashina.Extensions
{
    public static class SpeechSynthesisExtensions
    {
        /// <summary>
        /// Selects the voice by the language tag.
        /// </summary>
        /// <param name="synth">The <see cref="SpeechSynthesizer"/> instance.</param>
        /// <param name="language">The language tag.</param>
        /// <exception cref="ArgumentException">When the string is not valid.</exception>
        /// <exception cref="ArgumentNullException">When the language is null.</exception>
        /// <exception cref="CultureNotFoundException">When an invalid culture name is given.</exception>
        public static void SelectVoiceByLanguage(this SpeechSynthesizer synth, string language)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(language);

            CultureInfo cultureInfo = CultureInfo.GetCultureInfo(language);

            synth.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Teen, 0, cultureInfo);
        }

        /// <inheritdoc cref="SelectVoiceByLanguage(SpeechSynthesizer, string)"/>
        /// <param name="culture">The culture object.</param>
        public static void SelectVoiceByLanguage(this SpeechSynthesizer synth, CultureInfo culture)
        {
            ArgumentNullException.ThrowIfNull(culture);

            synth.SelectVoiceByHints(
                VoiceGender.Female,
                VoiceAge.Teen,
                0,
                culture);
        }
    }
}
