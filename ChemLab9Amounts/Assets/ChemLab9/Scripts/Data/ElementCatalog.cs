using System;
namespace ChemLab9.Data
{
    // Atomic weights: rounded conventional values, IUPAC/CIAAW table; not isotope mass numbers.
    public static class ElementCatalog
    {
        static ElementRecord E(string symbol, string name, int z, float mass, int group, int period, ElementCategory category, params int[] shells)
        {
            string[] old = { "IA", "IIA", "IIIA", "IVA", "VA", "VIA", "VIIA", "VIIIA" };
            int index = group <= 2 ? group - 1 : group - 11;
            bool hasRadius = symbol == "Li" || symbol == "Na" || symbol == "K" || symbol == "Mg" || symbol == "Al";
            return new ElementRecord { symbol = symbol, name = name, atomicNumber = z, atomicMass = mass, group = group, period = period, category = category, shells = shells, oldGroup = old[index], relativeRadius = hasRadius ? RelativeRadius(symbol) : 0, radiusNote = hasRadius ? "Mô hình minh họa, không theo tỉ lệ thực; không có đơn vị pm." : "Chưa triển khai bán kính." };
        }
        public static ElementRecord[] FirstTwenty() => new[] {
            E("H", "Hiđro", 1, 1.008f, 1, 1, ElementCategory.Nonmetal, 1),
            E("He", "Heli", 2, 4.0026f, 18, 1, ElementCategory.NobleGas, 2),
            E("Li", "Liti", 3, 6.94f, 1, 2, ElementCategory.AlkaliMetal, 2, 1),
            E("Be", "Beri", 4, 9.0122f, 2, 2, ElementCategory.AlkalineEarth, 2, 2),
            E("B", "Bo", 5, 10.81f, 13, 2, ElementCategory.Metalloid, 2, 3),
            E("C", "Cacbon", 6, 12.011f, 14, 2, ElementCategory.Nonmetal, 2, 4),
            E("N", "Nitơ", 7, 14.007f, 15, 2, ElementCategory.Nonmetal, 2, 5),
            E("O", "Oxi", 8, 15.999f, 16, 2, ElementCategory.Nonmetal, 2, 6),
            E("F", "Flo", 9, 18.9984f, 17, 2, ElementCategory.Halogen, 2, 7),
            E("Ne", "Neon", 10, 20.1797f, 18, 2, ElementCategory.NobleGas, 2, 8),
            E("Na", "Natri", 11, 22.9898f, 1, 3, ElementCategory.AlkaliMetal, 2, 8, 1),
            E("Mg", "Magie", 12, 24.305f, 2, 3, ElementCategory.AlkalineEarth, 2, 8, 2),
            E("Al", "Nhôm", 13, 26.9815f, 13, 3, ElementCategory.Metal, 2, 8, 3),
            E("Si", "Silic", 14, 28.085f, 14, 3, ElementCategory.Metalloid, 2, 8, 4),
            E("P", "Photpho", 15, 30.9738f, 15, 3, ElementCategory.Nonmetal, 2, 8, 5),
            E("S", "Lưu huỳnh", 16, 32.06f, 16, 3, ElementCategory.Nonmetal, 2, 8, 6),
            E("Cl", "Clo", 17, 35.45f, 17, 3, ElementCategory.Halogen, 2, 8, 7),
            E("Ar", "Argon", 18, 39.95f, 18, 3, ElementCategory.NobleGas, 2, 8, 8),
            E("K", "Kali", 19, 39.0983f, 1, 4, ElementCategory.AlkaliMetal, 2, 8, 8, 1),
            E("Ca", "Canxi", 20, 40.078f, 2, 4, ElementCategory.AlkalineEarth, 2, 8, 8, 2)
        };
        public static float RelativeRadius(string symbol)
        {
            switch (symbol) { case "Li": return 0.7f; case "Na": return 1f; case "K": return 1.3f; case "Mg": return 0.82f; case "Al": return 0.68f; default: throw new ArgumentException("No illustration for " + symbol); }
        }
        public static bool IsRadiusOrder(string[] symbols)
        {
            return symbols != null && symbols.Length == 3 && symbols[0] == "Li" && symbols[1] == "Na" && symbols[2] == "K";
        }
    }
}
