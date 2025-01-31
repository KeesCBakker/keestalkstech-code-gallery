using HandlebarsDotNet.Compiler;
using HandlebarsDotNet.Iterators;
using HandlebarsDotNet.ObjectDescriptors;
using HandlebarsDotNet.PathStructure;
using HandlebarsDotNet.Runtime;
using HandlebarsDotNet.ValueProviders;
using System.Numerics;
using System.Reflection;

namespace Ktt.JsonHandlebars;

public class FlaggedEnumObjectDescriptorProvider : IObjectDescriptorProvider
{
    public bool TryGetDescriptor(Type type, out ObjectDescriptor value)
    {
        if (!type.IsEnum || type.GetCustomAttribute<FlagsAttribute>() == null)
        {
            value = ObjectDescriptor.Empty;
            return false;
        }

        value = new ObjectDescriptor(
            type,
            null,
            null,
            self => new FlagEnumInterator()
        );

        return true;
    }

    public static string[] DeconstructFlags(Enum items)
    {
        if (items.GetType().GetCustomAttribute<FlagsAttribute>() == null)
        {
            throw new ArgumentException("Enum has no [Flags] attribute.", nameof(items));
        }

        // no value, no list
        var itemsValue = (int)(object)items;
        if (itemsValue == 0)
        {
            return [];
        }

        var result = new List<string>();

        foreach (var item in Enum.GetValues(items.GetType()))
        {
            if (item == null)
            {
                continue;
            }

            var value = (int)item;

            // skip combined flags
            if (!BitOperations.IsPow2(value))
            {
                continue;
            }

            if (item is Enum @enum && items.HasFlag(@enum))
            {
                result.Add(item.ToString() ?? "");
            }
        }

        return [.. result];

    }

    internal class FlagEnumInterator : IIterator
    {
        public void Iterate(in HandlebarsDotNet.EncodedTextWriter writer, HandlebarsDotNet.BindingContext context, ChainSegment[] blockParamsVariables, object input, TemplateDelegate template, TemplateDelegate ifEmpty)
        {
            using var innerContext = context.CreateFrame();
            var iterator = new IteratorValues(innerContext);
            var blockParamsValues = new BlockParamsValues(innerContext, blockParamsVariables);

            blockParamsValues.CreateProperty(0, out var _0);
            blockParamsValues.CreateProperty(1, out var _1);

            iterator.First = BoxedValues.True;
            iterator.Last = BoxedValues.False;

            var target = DeconstructFlags((Enum)input);

            var count = target.Length;
            var enumerator = target.GetEnumerator();

            var index = 0;
            var lastIndex = count - 1;
            while (enumerator.MoveNext())
            {
                var value = enumerator.Current;
                var objectIndex = BoxedValues.Int(index);

                if (index == 1)
                {
                    iterator.First = BoxedValues.False;
                }

                if (index == lastIndex)
                {
                    iterator.Last = BoxedValues.True;
                }

                iterator.Index = objectIndex;

                object resolvedValue = value;

                blockParamsValues[_0] = resolvedValue;
                blockParamsValues[_1] = objectIndex;

                iterator.Value = resolvedValue;
                innerContext.Value = resolvedValue;

                template(writer, innerContext);

                ++index;
            }

            if (index == 0)
            {
                innerContext.Value = context.Value;
                ifEmpty(writer, innerContext);
            }

        }
    }
}
