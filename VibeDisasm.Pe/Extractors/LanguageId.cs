namespace VibeDisasm.Pe.Extractors
{
    /// <summary>
    /// Represents language identifiers (LCID) used in Windows resources
    /// </summary>
    public enum LanguageId : uint
    {
        /// <summary>Neutral language</summary>
        Neutral = 0x0000,
        
        /// <summary>Arabic (Saudi Arabia)</summary>
        ArabicSaudiArabia = 0x0401,
        /// <summary>Bulgarian</summary>
        Bulgarian = 0x0402,
        /// <summary>Catalan</summary>
        Catalan = 0x0403,
        /// <summary>Chinese (Traditional, Taiwan)</summary>
        ChineseTraditionalTaiwan = 0x0404,
        /// <summary>Czech</summary>
        Czech = 0x0405,
        /// <summary>Danish</summary>
        Danish = 0x0406,
        /// <summary>German (Germany)</summary>
        GermanGermany = 0x0407,
        /// <summary>Greek</summary>
        Greek = 0x0408,
        /// <summary>English (United States)</summary>
        EnglishUS = 0x0409,
        /// <summary>Spanish (Traditional Sort)</summary>
        SpanishTraditionalSort = 0x040A,
        /// <summary>Finnish</summary>
        Finnish = 0x040B,
        /// <summary>French (France)</summary>
        FrenchFrance = 0x040C,
        /// <summary>Hebrew</summary>
        Hebrew = 0x040D,
        /// <summary>Hungarian</summary>
        Hungarian = 0x040E,
        /// <summary>Icelandic</summary>
        Icelandic = 0x040F,
        /// <summary>Italian (Italy)</summary>
        ItalianItaly = 0x0410,
        /// <summary>Japanese</summary>
        Japanese = 0x0411,
        /// <summary>Korean</summary>
        Korean = 0x0412,
        /// <summary>Dutch (Netherlands)</summary>
        DutchNetherlands = 0x0413,
        /// <summary>Norwegian (Bokmal)</summary>
        NorwegianBokmal = 0x0414,
        /// <summary>Polish</summary>
        Polish = 0x0415,
        /// <summary>Portuguese (Brazil)</summary>
        PortugueseBrazil = 0x0416,
        /// <summary>Romanian</summary>
        Romanian = 0x0418,
        /// <summary>Russian</summary>
        Russian = 0x0419,
        /// <summary>Croatian</summary>
        Croatian = 0x041A,
        /// <summary>Slovak</summary>
        Slovak = 0x041B,
        /// <summary>Albanian</summary>
        Albanian = 0x041C,
        /// <summary>Swedish</summary>
        Swedish = 0x041D,
        /// <summary>Thai</summary>
        Thai = 0x041E,
        /// <summary>Turkish</summary>
        Turkish = 0x041F,
        /// <summary>Urdu</summary>
        Urdu = 0x0420,
        /// <summary>Indonesian</summary>
        Indonesian = 0x0421,
        /// <summary>Ukrainian</summary>
        Ukrainian = 0x0422,
        /// <summary>Belarusian</summary>
        Belarusian = 0x0423,
        /// <summary>Slovenian</summary>
        Slovenian = 0x0424,
        /// <summary>Estonian</summary>
        Estonian = 0x0425,
        /// <summary>Latvian</summary>
        Latvian = 0x0426,
        /// <summary>Lithuanian</summary>
        Lithuanian = 0x0427,
        /// <summary>Persian</summary>
        Persian = 0x0429,
        /// <summary>Vietnamese</summary>
        Vietnamese = 0x042A,
        /// <summary>Armenian</summary>
        Armenian = 0x042B,
        /// <summary>Azeri (Latin)</summary>
        AzeriLatin = 0x042C,
        /// <summary>Basque</summary>
        Basque = 0x042D,
        /// <summary>Macedonian</summary>
        Macedonian = 0x042F,
        /// <summary>Afrikaans</summary>
        Afrikaans = 0x0436,
        /// <summary>Georgian</summary>
        Georgian = 0x0437,
        /// <summary>Faroese</summary>
        Faroese = 0x0438,
        /// <summary>Hindi</summary>
        Hindi = 0x0439,
        /// <summary>Malay (Malaysia)</summary>
        MalayMalaysia = 0x043E,
        /// <summary>Kazakh</summary>
        Kazakh = 0x043F,
        /// <summary>Kyrgyz</summary>
        Kyrgyz = 0x0440,
        /// <summary>Swahili</summary>
        Swahili = 0x0441,
        /// <summary>Uzbek (Latin)</summary>
        UzbekLatin = 0x0443,
        /// <summary>Tatar</summary>
        Tatar = 0x0444,
        /// <summary>Bengali</summary>
        Bengali = 0x0445,
        /// <summary>Punjabi</summary>
        Punjabi = 0x0446,
        /// <summary>Gujarati</summary>
        Gujarati = 0x0447,
        /// <summary>Oriya</summary>
        Oriya = 0x0448,
        /// <summary>Tamil</summary>
        Tamil = 0x0449,
        /// <summary>Telugu</summary>
        Telugu = 0x044A,
        /// <summary>Kannada</summary>
        Kannada = 0x044B,
        /// <summary>Malayalam</summary>
        Malayalam = 0x044C,
        /// <summary>Assamese</summary>
        Assamese = 0x044D,
        /// <summary>Marathi</summary>
        Marathi = 0x044E,
        /// <summary>Sanskrit</summary>
        Sanskrit = 0x044F,
        /// <summary>Mongolian</summary>
        Mongolian = 0x0450,
        /// <summary>Chinese (Simplified, PRC)</summary>
        ChineseSimplifiedPRC = 0x0804,
        /// <summary>German (Switzerland)</summary>
        GermanSwitzerland = 0x0807,
        /// <summary>English (United Kingdom)</summary>
        EnglishUK = 0x0809,
        /// <summary>Spanish (Mexico)</summary>
        SpanishMexico = 0x080A,
        /// <summary>French (Belgium)</summary>
        FrenchBelgium = 0x080C,
        /// <summary>Italian (Switzerland)</summary>
        ItalianSwitzerland = 0x0810,
        /// <summary>Dutch (Belgium)</summary>
        DutchBelgium = 0x0813,
        /// <summary>Norwegian (Nynorsk)</summary>
        NorwegianNynorsk = 0x0814,
        /// <summary>Portuguese (Portugal)</summary>
        PortuguesePortugal = 0x0816,
        /// <summary>Serbian (Latin)</summary>
        SerbianLatin = 0x081A,
        /// <summary>Swedish (Finland)</summary>
        SwedishFinland = 0x081D,
        /// <summary>Azeri (Cyrillic)</summary>
        AzeriCyrillic = 0x082C,
        /// <summary>Malay (Brunei Darussalam)</summary>
        MalayBruneiDarussalam = 0x083E,
        /// <summary>Uzbek (Cyrillic)</summary>
        UzbekCyrillic = 0x0843,
        /// <summary>Chinese (Traditional, Hong Kong S.A.R.)</summary>
        ChineseTraditionalHongKong = 0x0C04,
        /// <summary>German (Austria)</summary>
        GermanAustria = 0x0C07,
        /// <summary>English (Australia)</summary>
        EnglishAustralia = 0x0C09,
        /// <summary>Spanish (International Sort)</summary>
        SpanishInternationalSort = 0x0C0A,
        /// <summary>French (Canada)</summary>
        FrenchCanada = 0x0C0C,
        /// <summary>Serbian (Cyrillic)</summary>
        SerbianCyrillic = 0x0C1A,
        /// <summary>Chinese (Simplified, Singapore)</summary>
        ChineseSimplifiedSingapore = 0x1004,
        /// <summary>German (Luxembourg)</summary>
        GermanLuxembourg = 0x1007,
        /// <summary>English (Canada)</summary>
        EnglishCanada = 0x1009,
        /// <summary>Spanish (Guatemala)</summary>
        SpanishGuatemala = 0x100A,
        /// <summary>French (Switzerland)</summary>
        FrenchSwitzerland = 0x100C,
        /// <summary>Chinese (Traditional, Macau S.A.R.)</summary>
        ChineseTraditionalMacau = 0x1404,
        /// <summary>German (Liechtenstein)</summary>
        GermanLiechtenstein = 0x1407,
        /// <summary>English (New Zealand)</summary>
        EnglishNewZealand = 0x1409,
        /// <summary>Spanish (Costa Rica)</summary>
        SpanishCostaRica = 0x140A,
        /// <summary>French (Luxembourg)</summary>
        FrenchLuxembourg = 0x140C,
        /// <summary>English (Ireland)</summary>
        EnglishIreland = 0x1809,
        /// <summary>Spanish (Panama)</summary>
        SpanishPanama = 0x180A,
        /// <summary>French (Monaco)</summary>
        FrenchMonaco = 0x180C,
        /// <summary>English (South Africa)</summary>
        EnglishSouthAfrica = 0x1C09,
        /// <summary>Spanish (Dominican Republic)</summary>
        SpanishDominicanRepublic = 0x1C0A,
        /// <summary>English (Jamaica)</summary>
        EnglishJamaica = 0x2009,
        /// <summary>Spanish (Venezuela)</summary>
        SpanishVenezuela = 0x200A,
        /// <summary>English (Caribbean)</summary>
        EnglishCaribbean = 0x2409,
        /// <summary>Spanish (Colombia)</summary>
        SpanishColombia = 0x240A,
        /// <summary>English (Belize)</summary>
        EnglishBelize = 0x2809,
        /// <summary>Spanish (Peru)</summary>
        SpanishPeru = 0x280A,
        /// <summary>English (Trinidad and Tobago)</summary>
        EnglishTrinidadAndTobago = 0x2C09,
        /// <summary>Spanish (Argentina)</summary>
        SpanishArgentina = 0x2C0A,
        /// <summary>English (Zimbabwe)</summary>
        EnglishZimbabwe = 0x3009,
        /// <summary>Spanish (Ecuador)</summary>
        SpanishEcuador = 0x300A,
        /// <summary>English (Philippines)</summary>
        EnglishPhilippines = 0x3409,
        /// <summary>Spanish (Chile)</summary>
        SpanishChile = 0x340A,
        /// <summary>Spanish (Uruguay)</summary>
        SpanishUruguay = 0x380A,
        /// <summary>Spanish (Paraguay)</summary>
        SpanishParaguay = 0x3C0A,
        /// <summary>Spanish (Bolivia)</summary>
        SpanishBolivia = 0x400A,
        /// <summary>Spanish (El Salvador)</summary>
        SpanishElSalvador = 0x440A,
        /// <summary>Spanish (Honduras)</summary>
        SpanishHonduras = 0x480A,
        /// <summary>Spanish (Nicaragua)</summary>
        SpanishNicaragua = 0x4C0A,
        /// <summary>Spanish (Puerto Rico)</summary>
        SpanishPuertoRico = 0x500A,
    }
}
