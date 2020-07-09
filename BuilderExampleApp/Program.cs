using System;
using System.Collections.Generic;

namespace BuilderExampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var value = new ConcreteEntityBuilder()
                .AddConcretePropertyA("Prop A")
                .AddConcretePropertyB("Prop B")
                .Build();

            (value as ConcreteEntity).Process();
        }
    }


    public interface IProperty
    {
        object Value { get; set; }
    }

    /* Целевая сущность может быть абсолютно разной. Тут я её сделал пустой,
     * но для реальных задач можно предпологать, что в целевой абстрактной 
     * сущности будут свойства абстрактных типов и т.д.
     */
    public interface IEntity
    { }

    // builder interface
    public interface IEntityBuilder
    {
        IEntityBuilder Add(IProperty property);
        IEntity Build();
    }

    public class ConcreteEntityBuilder : IEntityBuilder
    {
        // будет хранить название типов и их значение
        Dictionary<string, object> _concreteValues;

        public ConcreteEntityBuilder()
        {
            _concreteValues = new Dictionary<string, object>();
        }

        // узнаёт у параметра название его конкретного типа
        // и добавляет в словарь его название и значение
        public IEntityBuilder Add(IProperty property)
        {
            var propType = property.GetType();
            _concreteValues.Add(propType.Name, property.Value);
            return this;
        }

        // Инициализирует свойства конкретной сущности и возвращает её
        public IEntity Build() 
        {
            var target = new ConcreteEntity();
            foreach (var pair in _concreteValues)
            {
                if (pair.Key == target.PropertyA.GetType().Name)
                    target.PropertyA = new ConcretePropertyA() { Value = pair.Value };
                if (pair.Key == target.PropertyB.GetType().Name)
                    target.PropertyB = new ConcretePropertyB() { Value = pair.Value };
            }
            
            return target;
        }
    }

    // Пример реализации конкретной сущности
    public class ConcreteEntity : IEntity
    {
        public ConcretePropertyA PropertyA { get; set; }
        public ConcretePropertyB PropertyB { get; set; }

        public ConcreteEntity()
        {
            PropertyA = new ConcretePropertyA();
            PropertyB = new ConcretePropertyB();
        }

        public void Process()
        {
            System.Console.WriteLine("Property A: " + PropertyA?.Value);
            System.Console.WriteLine("Property B: " + PropertyB?.Value);
        }
    }

    /* Конкретные свойства для целевой сущности могут
     * поставляться из других проектов. Если бизнес логика
     * нашей конкретной сущности требует в кач-ве свойства
     * именно эти типы, то для них можно написать extension
     * методы 
     */ 
    public class ConcretePropertyA : IProperty
    {
        public object Value { get; set; }
    }

    public class ConcretePropertyB : IProperty
    {
        public object Value { get; set; }
    }

    public static class ConcretePropertyBFactory
    {
        public static IEntityBuilder AddConcretePropertyB(this IEntityBuilder builder, object value) =>
            builder.Add(new ConcretePropertyB() { Value = value });
    
    }

    public static class ConcretePropertyAFactory
    {
        public static IEntityBuilder AddConcretePropertyA(this IEntityBuilder builder, object value) =>
            builder.Add(new ConcretePropertyA() { Value = value });
    }
}
