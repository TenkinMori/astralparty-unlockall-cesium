using System;
using System.Collections;
using System.Reflection;
using CesiumLoader.SDK.Configuration;
using CesiumLoader.SDK.Mods;
using GameLogic;
using Tools;

namespace UnlockAllCharacters
{
    public static class ModEntry
    {
        public static void Main()
        {
            ModBase.Run(new UnlockAllCharactersController());
        }
    }

    /// <summary>
    /// 全角色解锁：不修改存档，不伪造 RoleCard，只在客户端把 HeroCardData._IsHas
    /// 改成 true。HeroCardData 的 IsHas / heroStatus 都读这个字段，所以角色列表、
    /// 详情、选人界面会显示为已解锁。
    ///
    /// 注意：这是客户端效果，只影响本地 UI；服务器仍然按账号真实角色校验。
    /// </summary>
    public sealed class UnlockAllCharactersController : ModBase
    {
        private const double PollSeconds = 1.0;
        private const BindingFlags PrivateInstance = BindingFlags.Instance | BindingFlags.NonPublic;

        private bool _enabled = true;
        private double _nextPoll;
        private int _lastPatchedCount = -1;

        public override string Name { get { return "解锁全角色"; } }
        public override string Version { get { return "1.0.0"; } }
        public override string Author { get { return "TenkinMori"; } }
        public override string Description
        {
            get { return "提示：请勿在使用本插件的情况下进行抽卡，后果自负！！！\n解锁未解锁的角色，但PVE等级固定为0级。推荐在PVP等无等级的模式中使用，请低调使用。"; }
        }

        public override void OnInitialize()
        {
            ReadConfig(true);
            Log.Info("配置: enabled=" + _enabled);
        }

        public override void OnConfigChanged()
        {
            ReadConfig(false);
            Log.Info("配置已更新: enabled=" + _enabled);
        }

        public override void OnUpdate()
        {
            if (!_enabled) return;
            double now = DateTime.UtcNow.TimeOfDay.TotalSeconds;
            if (now < _nextPoll) return;
            _nextPoll = now + PollSeconds;

            try { Patch(); }
            catch (Exception e) { Warn("解锁失败: " + e.Message); }
        }

        private void Patch()
        {
            GameLogicManager manager = SimpleSingletonProvider<GameLogicManager>.inst;
            if (manager == null) return;

            object heroCardManager = manager.heroCard;
            if (heroCardManager == null) return;

            FieldInfo dictField = heroCardManager.GetType().GetField("heroCardDict", PrivateInstance);
            if (dictField == null) return;

            IDictionary dict = dictField.GetValue(heroCardManager) as IDictionary;
            if (dict == null) return;

            int patched = 0;
            foreach (DictionaryEntry entry in dict)
            {
                object card = entry.Value;
                if (card == null) continue;

                FieldInfo hasField = card.GetType().GetField("_IsHas", PrivateInstance);
                if (hasField == null) continue;

                if (!(bool)hasField.GetValue(card))
                {
                    hasField.SetValue(card, true);
                    patched++;
                }
            }

            int total = dict.Count;
            if (patched > 0 || total != _lastPatchedCount)
            {
                _lastPatchedCount = total;
                Log.Info("全角色解锁: " + total + " 个角色（本次修改 " + patched + "）");
            }
        }

        private void Warn(string message)
        {
            Log.Info("[WARN] " + message);
        }

        private void ReadConfig(bool writeDefaults)
        {
            ModConfig cfg = Config;
            if (cfg == null) return;

            if (writeDefaults && cfg.Count == 0)
            {
                cfg.Set("enabled", true);
                cfg.SaveIfDirty();
            }

            _enabled = cfg.GetBool("enabled", true);
        }
    }
}
