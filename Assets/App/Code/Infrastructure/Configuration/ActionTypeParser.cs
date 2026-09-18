using System;
using System.Collections.Generic;
using System.Text;
using App.Domain.Actions;

namespace App.Infrastructure.Configuration
{
    /// <summary>
    /// Разбирает название действия из конфигурации в доменный тип.
    ///
    /// Понимает и русские названия из технического задания ("подойти", "граб",
    /// "нажать кнопку"), и английские имена элементов перечисления — автор
    /// сценария не должен подстраиваться под внутренние имена кода.
    /// </summary>
    public static class ActionTypeParser
    {
        /// <summary>
        /// Пары "название — тип действия". Названия проходят ту же нормализацию,
        /// что и значение из файла, поэтому записывать их здесь можно естественно.
        /// </summary>
        private static readonly KeyValuePair<string, ActionType>[] RawAliases =
        {
            new KeyValuePair<string, ActionType>("подойти", ActionType.ReachPoint),
            new KeyValuePair<string, ActionType>("дойти", ActionType.ReachPoint),
            new KeyValuePair<string, ActionType>("дойти до точки", ActionType.ReachPoint),
            new KeyValuePair<string, ActionType>("reach", ActionType.ReachPoint),
            new KeyValuePair<string, ActionType>("reachpoint", ActionType.ReachPoint),

            new KeyValuePair<string, ActionType>("граб", ActionType.GrabObject),
            new KeyValuePair<string, ActionType>("взять", ActionType.GrabObject),
            new KeyValuePair<string, ActionType>("взять объект", ActionType.GrabObject),
            new KeyValuePair<string, ActionType>("grab", ActionType.GrabObject),
            new KeyValuePair<string, ActionType>("grabobject", ActionType.GrabObject),

            new KeyValuePair<string, ActionType>("клик", ActionType.ClickObject),
            new KeyValuePair<string, ActionType>("кликнуть", ActionType.ClickObject),
            new KeyValuePair<string, ActionType>("кликнуть на объект", ActionType.ClickObject),
            new KeyValuePair<string, ActionType>("луч", ActionType.ClickObject),
            new KeyValuePair<string, ActionType>("click", ActionType.ClickObject),
            new KeyValuePair<string, ActionType>("clickobject", ActionType.ClickObject),

            new KeyValuePair<string, ActionType>("нажать", ActionType.PressUiButton),
            new KeyValuePair<string, ActionType>("нажать кнопку", ActionType.PressUiButton),
            new KeyValuePair<string, ActionType>("нажать на кнопку ui", ActionType.PressUiButton),
            new KeyValuePair<string, ActionType>("кнопка", ActionType.PressUiButton),
            new KeyValuePair<string, ActionType>("button", ActionType.PressUiButton),
            new KeyValuePair<string, ActionType>("pressuibutton", ActionType.PressUiButton)
        };

        private static readonly Dictionary<string, ActionType> Aliases = BuildAliases();

        /// <summary>Разобрать название действия.</summary>
        /// <exception cref="FormatException">Название не указано или не распознано.</exception>
        public static ActionType Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new FormatException("Не указано ожидаемое действие.");

            if (Aliases.TryGetValue(Normalize(value), out var actionType))
                return actionType;

            throw new FormatException(
                "Неизвестное действие \"" + value + "\". Допустимы: подойти, граб, клик, нажать кнопку.");
        }

        private static Dictionary<string, ActionType> BuildAliases()
        {
            var aliases = new Dictionary<string, ActionType>(RawAliases.Length, StringComparer.Ordinal);

            foreach (var alias in RawAliases)
                aliases[Normalize(alias.Key)] = alias.Value;

            return aliases;
        }

        /// <summary>
        /// Приводит запись к единому виду: нижний регистр, одиночные пробелы,
        /// "ё" как "е" и без предлогов, которые автор сценария может написать
        /// по-разному ("кликнуть на объект" и "кликнуть по объекту" — одно и то же).
        /// </summary>
        private static string Normalize(string value)
        {
            var builder = new StringBuilder(value.Length);
            var previousWasSpace = true;

            foreach (var symbol in value.Trim().ToLowerInvariant())
            {
                var current = symbol == 'ё' ? 'е' : symbol;

                if (char.IsWhiteSpace(current))
                {
                    if (previousWasSpace)
                        continue;

                    previousWasSpace = true;
                    builder.Append(' ');
                    continue;
                }

                previousWasSpace = false;
                builder.Append(current);
            }

            return RemovePrepositions(builder.ToString().Trim());
        }

        private static string RemovePrepositions(string value)
        {
            var words = value.Split(' ');
            var kept = new List<string>(words.Length);

            foreach (var word in words)
            {
                if (word == "на" || word == "по" || word == "в" || word == "до" || word == "к")
                    continue;

                kept.Add(word);
            }

            return string.Join(" ", kept);
        }
    }
}
