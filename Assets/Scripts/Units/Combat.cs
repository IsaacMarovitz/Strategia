using System;

public static class Combat {
    private static Random random = new Random();

    // Returns (false, -) if attack missed
    // Returns (true, -) if attack hit
    public static async (bool, double) CalculateDamage(CombatInfo atkInfo, CombatInfo defInfo) {
        if (random.NextDouble() > atkInfo.precision) {
            // Attack missed
            return (false, 0);
        }

        int atk = atkInfo.baseAtk;

        if (random.NextDouble() < atkInfo.atkCrit) {
            atk += atkInfo.atkCritBonus;
        }

        int def = defInfo.baseDef;

        if (random.NextDouble() < defInfo.defCrit) {
            def += defInfo.defCritBonus;
        }

        double accuracy = random.NextDouble() * (atkInfo.accuracy * 2) - atkInfo.accuracy;
        double damage = 30 * Math.Exp(((def - atk) / 25 * accuracy));

        return (true, damage);
    }
}

public class CombatInfo {
    public int baseAtk = 30;
    public double atkCrit = 0.33;
    public int atkCritBonus = 20;

    public int baseDef = 15;
    public double defCrit = 0.16;
    public int defCritBonus = 40;

    public double accuracy = 0.2;
    public double precision = 0.95;
}