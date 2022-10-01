using Bygdrift.Tools.CsvTool;
using Bygdrift.Warehouse;

namespace Module.Refines
{
    public class EK109ExplanationTables
    {
        public static void Refine(AppBase<Settings> app)
        {
            app.Mssql.InsertCsv(Typekode(), "Typekode", false, false);
            app.Mssql.InsertCsv(Energiartskode(), "Energiartskode", false, false);
            app.Mssql.InsertCsv(Artskode(), "Artskode", false, false);
            app.Mssql.InsertCsv(Enhedskode(), "Enhedskode", false, false);
        }

        private static Csv Typekode()
        {
            return new Csv("Kode, Type, Beskrivelse")
                .AddRow(1, "Tællerstand", "Aflæst tællerstand")
                .AddRow(1, "Tællerstand", "Aflæst tællerstand")
                .AddRow(2, "Forbrugsmåling", "Forbrug/produktion for en periode (Forbrugsmåling/faktor kræver fra tidspunkt).")
                .AddRow(3, "Aktuel værdi", "Aflæst aktuel/øjebliksværdi")
                .AddRow(4, "Andet")
                .AddRow(5, "Tank pejling")
                .AddRow(6, "Tank påfyldning")
                .AddRow(10, "Faktor", "Benyttes til f.eks.variabel brændværdi for en periode (*1)");
        }

        private static Csv Energiartskode()
        {
            return new Csv("Kode, Energiart, Beskrivelse")
                .AddRow(1, "El")
                .AddRow(2, "Vand")
                .AddRow(3, "Centralvarme ", "Centralvarme benyttes til varmemålere på sekundær side / egen varmecentral (gas/olie fyr) / fordelingsmåler mv.")
                .AddRow(4, "Fjernvarme ", "Fjernvarme benyttes til varmemålere på primærside / tilgang direkte tilkoblet fjernvarme nettet.")
                .AddRow(5, "Olie")
                .AddRow(6, "Gas ", "Benyttes til bygas/naturgas/biogas")
                .AddRow(7, "Køling")
                .AddRow(8, "Trykluft", "Benyttes til målt luftmængde fra kompressor mv.");
        }

        private static Csv Artskode()
        {
            return new Csv("Kode, Målepunktsart, Beskrivelse")
                .AddRow(1, "Energi ", "Aflæst energi eller energi forbrug/produktion for en periode jf. typekoden.")
                .AddRow(2, "Volumen ", "Aflæst volumen eller volumen forbrug/produktion for en periode jf. typekoden.")
                .AddRow(3, "Timer ", "Aflæst timetæller")
                .AddRow(4, "Fremløbstemperatur ", "Aflæst fremløbstemperatur")
                .AddRow(5, "Returløbstemperatur ", "Aflæst returløbstemperatur")
                .AddRow(6, "Temperatur forskel ", "Aflæst fremløbs- minus returløbstemperatur")
                .AddRow(7, "Aktuel effekt ", "Aflæst effekt")
                .AddRow(8, "Aktuel flow ", "Aflæst flow")
                .AddRow(9, "Maks. effekt ", "Aflæst maks. effekt")
                .AddRow(10, "Maks. flow ", "Aflæst maks. flow")
                .AddRow(11, "Fremført energi (volumen*temp.)", "Aflæst fremført energi som m3*c")
                .AddRow(12, "Tilbageført energi (volumen*temp.)", "Aflæst tilbage energi som m3*c")
                .AddRow(13, "Fremført energi (energi) ", "Aflæst fremført energi som MWh, Gj mv.")
                .AddRow(14, "Tilbageført energi (energi) ", "Aflæst tilbageført energi som MWh, Gj mv.")
                .AddRow(15, "Min. flow ", "Aflæst min. flow")
                .AddRow(16, "Tryk 1 ", "Aflæst tryk")
                .AddRow(17, "Tryk 2 ", "Aflæst tryk")
                .AddRow(18, "Masse ", "Aflæst masse")
                .AddRow(19, "Solgt energi ", "Aflæst solgt energi")
                .AddRow(30, "Elektrisk spænding 1 ", "Aflæst volt for fase 1 ")
                .AddRow(31, "Elektrisk spænding 2 ", "Aflæst volt for fase 2 ")
                .AddRow(32, "Elektrisk spænding 3 ", "Aflæst volt for fase 3 ")
                .AddRow(33, "Elektrisk strømstyrke 1 ", "Aflæst ampere for fase 1 ")
                .AddRow(34, "Elektrisk strømstyrke 2 ", "Aflæst ampere for fase 2 ")
                .AddRow(35, "Elektrisk strømstyrke 3 ", "Aflæst ampere for fase 3 ")
                .AddRow(50, "Flow normal ", "Benyttes ved Gas flow computer")
                .AddRow(51, "Flow korrigeret ", "Benyttes ved Gas flow computer")
                .AddRow(52, "Flow ukorrigeret ", "Benyttes ved Gas flow computer")
                .AddRow(53, "Flow kontrol ukorrigeret ", "Benyttes ved Gas flow computer kontrolmåler")
                .AddRow(54, "Gas temperatur ", "Benyttes ved Gas flow computer")
                .AddRow(55, "Korrektions faktor ", "Benyttes ved Gas flow computer")
                .AddRow(101, "Værdi 1 ", "Værdi 1, kan benyttes til andre/ukendte arter")
                .AddRow(100, "Værdi N ", "N: 1-45 Værdi N, kan benyttes til andre/ukendte arter")
                .AddRow(201, "Faktor ", "Faktor/konstant F.eks. gas brændværdi")
                .AddRow(202, "Udetemperatur ", "Udetemperatur sensor")
                .AddRow(203, "Luftfugtighed ", "Luftfugtighed sensor")
                .AddRow(204, "Inde temperatur ", "Indendørs temperatur")
                .AddRow(205, "Min. Temperatur ", "Minimum temperatur")
                .AddRow(206, "Maks. temperatur ", "Maksimum temperatur")
                .AddRow(207, "Udetemperatur gns. ", "Gennemsnitlig udetemperatur")
                .AddRow(208, "Nedbør ", "Nedbør")
                .AddRow(209, "Stråling ", "Stråling")
                .AddRow(210, "Solskin ", "Solskin")
                .AddRow(211, "Vindretning ", "Vindretning")
                .AddRow(212, "Vindstyrke ", "Vindstyrke")
                .AddRow(213, "Ventil - varme ", "Ventil – varme")
                .AddRow(214, "Ventil - køling ", "Ventil – køling")
                .AddRow(215, "Driftsstatus - on/off ", "Driftsstatus - on/off")
                .AddRow(216, "Driftsstatus - variabel ", "Driftsstatus – variabel")
                .AddRow(217, "Fremløbstemperatur (sekundær)", "Fremløbstemperatur (sekundær)")
                .AddRow(218, "Tilbageløbstemperatur (sekundær)", "Tilbageløbstemperatur (sekundær)")
                .AddRow(998, "Fejlkode ", "Typisk 16/32-bits maske som talværdi.")
                .AddRow(999, "Infokode ", "Typisk 16/32-bits maske som talværdi.");
        }

        private static Csv Enhedskode()
        {
            return new Csv("Enhedskode, Enhed, Beskrivelse")
                .AddRow("wh", "Watt timer", "Energi")
                .AddRow("kWh", "Kilo watt timer", "Energi")
                .AddRow("mWh", "Mega watt timer", "Energi")
                .AddRow("j", "Joule")
                .AddRow("kj", "Kilo joule")
                .AddRow("mj", "Mega joule")
                .AddRow("gj", "Giga joule")
                .AddRow("m3", "Kubikmeter")
                .AddRow("l", "Liter")
                .AddRow("c", "Celcius", "°-tegn er valgfrit")
                .AddRow("°c", "Celcius", "°-tegn er valgfrit")
                .AddRow("k", "Kelvin", "°-tegn er valgfrit")
                .AddRow("°k", "Kelvin", "°-tegn er valgfrit")
                .AddRow("h", "Timer", "Lille ’t’ tolkes som timer")
                .AddRow("t", "Timer", "Lille ’t’ tolkes som timer")
                .AddRow("d", "Dage")
                .AddRow("min", "Minutter")
                .AddRow("w", "Watt", "Effekt")
                .AddRow("kw", "Kilo watt", "Effekt")
                .AddRow("mw", "Mega watt", "Effekt")
                .AddRow("stk", "Styk")
                .AddRow("pcs", "Styk")
                .AddRow("%", "Procent")
                .AddRow("pct", "Procent")
                .AddRow("bar", "Bar")
                .AddRow("hPa", "Hektopascal")
                .AddRow("g", "Gram")
                .AddRow("kg", "Kilogram")
                .AddRow("T", "Ton", "Stort ’T’ tolkes som ton")
                .AddRow("ton", "Ton", "Stort ’T’ tolkes som ton")
                .AddRow("m3°c", "Kubikmeter gange celsius", "Benyttes ved målepunktsart fremført-/tilbageført energi")
                .AddRow("m3*°c", "Kubikmeter gange celsius", "Benyttes ved målepunktsart fremført-/tilbageført energi")
                .AddRow("m3x°c", "Kubikmeter gange celsius", "Benyttes ved målepunktsart fremført-/tilbageført energi")
                .AddRow("l/t", "Liter pr time", "Flow")
                .AddRow("l/h", "Liter pr time", "Flow")
                .AddRow("m3/t", "Kubikmeter pr time", "Flow")
                .AddRow("m3/h", "Kubikmeter pr time", "Flow")
                .AddRow("c", "Faktor/konstant", "Benyttes ved målepunktsart faktor til faktor-enhed (og ikke celsius/kelvin)")
                .AddRow("k", "Faktor/konstant", "Benyttes ved målepunktsart faktor til faktor-enhed (og ikke celsius/kelvin)")
                .AddRow("kWh/kg", "Energi (watt timer) pr masse/vægt", "Benyttes ved målepunktsart faktor til faktor-enhed")
                .AddRow("kWh/g", "Energi (watt timer) pr masse/vægt", "Benyttes ved målepunktsart faktor til faktor-enhed")
                .AddRow("gj/kg", "Energi (joule) pr masse/vægt", "Benyttes ved målepunktsart faktor til faktor-enhed")
                .AddRow("gj/g", "Energi (joule) pr masse/vægt", "Benyttes ved målepunktsart faktor til faktor-enhed")
                .AddRow("kWh/m3", "Energi (watt timer) pr volumen", "Benyttes ved målepunktsart faktor til faktor-enhed")
                .AddRow("kWh/l", "Energi (watt timer) pr volumen", "Benyttes ved målepunktsart faktor til faktor-enhed")
                .AddRow("kj/m3", "Energi (joule) pr volumen", "Benyttes ved målepunktsart faktor til faktor-enhed")
                .AddRow("kj/l", "Energi (joule) pr volumen", "Benyttes ved målepunktsart faktor til faktor-enhed")
                .AddRow("V", "Volt", "Elektrisk spænding")
                .AddRow("A", "Ampere", "Elektrisk strømstyrke")
                .AddRow("degree", "Grader", "Benyttes ved målepunktsart Vindretning")
                .AddRow("°", "Grader", "Benyttes ved målepunktsart Vindretning")
                .AddRow("m/s", "Meter pr sekund")
                .AddRow("mm", "Millimeter")
                .AddRow("W/m²", "Watt pr kvadratmeter");
        }
    }
}
