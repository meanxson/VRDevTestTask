using System;

namespace App.Infrastructure.Configuration
{
    /// <summary>
    /// Схема JSON-файла сценария. Отдельные классы-переносчики нужны потому,
    /// что доменные типы неизменяемы и проверяют свои данные в конструкторе,
    /// а JsonUtility умеет заполнять только открытые поля.
    ///
    /// Формат следует образцу из технического задания и добавляет к нему
    /// массив групп и необязательный список из нескольких ожидаемых действий:
    ///
    /// {
    ///   "title": "Досмотр на КПП",
    ///   "groups": [
    ///     {
    ///       "title": "Проверка документов",
    ///       "briefing": "Подойдите к стойке, возьмите паспорт и подтвердите проверку.",
    ///       "steps": [
    ///         { "id": 1, "description": "...", "expectedAction": "подойти", "target": "counter_zone" },
    ///         { "id": 2, "description": "...", "expectedActions": [ { "action": "подойти", "target": "..." } ] }
    ///       ]
    ///     }
    ///   ]
    /// }
    /// </summary>
    [Serializable]
    public sealed class ScenarioDto
    {
        public string title;
        public StepGroupDto[] groups;
    }

    /// <summary>Группа шагов в JSON-файле.</summary>
    [Serializable]
    public sealed class StepGroupDto
    {
        public string title;

        /// <summary>
        /// Необязательное сообщение о порядке ожидаемых действий. Если его нет,
        /// текст собирается из описаний шагов.
        /// </summary>
        public string briefing;

        public StepDto[] steps;
    }

    /// <summary>Шаг в JSON-файле.</summary>
    [Serializable]
    public sealed class StepDto
    {
        public int id;
        public string description;

        /// <summary>Краткая запись одного ожидаемого действия.</summary>
        public string expectedAction;

        /// <summary>Цель краткой записи.</summary>
        public string target;

        /// <summary>
        /// Развёрнутая запись, когда шаг ждёт нескольких действий сразу
        /// (например, подойти в зону и нажать кнопку). Имеет приоритет
        /// над краткой записью.
        /// </summary>
        public ExpectedActionDto[] expectedActions;
    }

    /// <summary>Одно ожидаемое действие в развёрнутой записи шага.</summary>
    [Serializable]
    public sealed class ExpectedActionDto
    {
        public string action;
        public string target;
    }
}
