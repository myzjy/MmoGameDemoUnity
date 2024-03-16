GameConfig = {}
-- 属性名字
GameConfig.PosGrowthTypeNum = {
    atk = 1,
    atkack = 2,
    criticalHitChance = 3,
    hp = 4,
    vitality = 5,
    defensiveValue = 6,
    defensive = 7,
    elementMastery = 8,
    chargingEfficiencyOfElements = 9,
    criticalHitDamage = 10,
}

-- 属性 type 对应的 名字
GameConfig.PosGrowthTypeName = {
    [GameConfig.PosGrowthTypeNum.atk] = "攻击力",
    [GameConfig.PosGrowthTypeNum.atkack] = "攻击力",
    [GameConfig.PosGrowthTypeNum.criticalHitChance] = "暴击率",
    [GameConfig.PosGrowthTypeNum.hp] = "生命值",
    [GameConfig.PosGrowthTypeNum.vitality] = "生命值",
    [GameConfig.PosGrowthTypeNum.defensiveValue] = "防御力",
    [GameConfig.PosGrowthTypeNum.defensive] = "防御力",
    [GameConfig.PosGrowthTypeNum.elementMastery] = "元素精通",
    [GameConfig.PosGrowthTypeNum.chargingEfficiencyOfElements] = "元素充能效率",
    [GameConfig.PosGrowthTypeNum.criticalHitDamage] = "暴击伤害",

}
