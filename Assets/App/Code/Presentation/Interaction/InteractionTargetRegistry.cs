using System.Collections.Generic;

namespace App.Presentation.Interaction
{
    /// <summary>
    /// Реестр целей сцены: по идентификатору из сценария отдаёт объекты, которые
    /// его носят. Нужен подсветке, чтобы не перебирать сцену поиском на каждом шаге.
    ///
    /// Цели регистрируются сами при включении и снимаются при выключении, поэтому
    /// реестр не переживает выгрузку сцены и не удерживает уничтоженные объекты.
    /// Один идентификатор могут носить несколько объектов — например, зона и её
    /// указатель, которые подсвечиваются вместе.
    /// </summary>
    public static class InteractionTargetRegistry
    {
        private static readonly Dictionary<string, List<InteractionTarget>> Targets =
            new Dictionary<string, List<InteractionTarget>>(System.StringComparer.OrdinalIgnoreCase);

        public static void Register(InteractionTarget target)
        {
            if (target == null || string.IsNullOrWhiteSpace(target.TargetId))
                return;

            if (!Targets.TryGetValue(target.TargetId, out var group))
            {
                group = new List<InteractionTarget>(1);
                Targets.Add(target.TargetId, group);
            }

            if (!group.Contains(target))
                group.Add(target);
        }

        public static void Unregister(InteractionTarget target)
        {
            if (target == null || string.IsNullOrWhiteSpace(target.TargetId))
                return;

            if (!Targets.TryGetValue(target.TargetId, out var group))
                return;

            group.Remove(target);

            if (group.Count == 0)
                Targets.Remove(target.TargetId);
        }

        /// <summary>Найти цели по идентификатору из сценария.</summary>
        public static bool TryGet(string targetId, out IReadOnlyList<InteractionTarget> targets)
        {
            if (!string.IsNullOrWhiteSpace(targetId) && Targets.TryGetValue(targetId, out var group))
            {
                targets = group;
                return true;
            }

            targets = null;
            return false;
        }
    }
}
