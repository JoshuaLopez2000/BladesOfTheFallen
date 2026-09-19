using System.Collections.Generic;
using NUnit.Framework;

public sealed class ListExtensionsTests
{
    [Test]
    public void Shuffle_PreservesEveryElement()
    {
        List<int> values = new() { 1, 2, 3, 4, 5 };

        values.Shuffle();

        Assert.That(values, Is.EquivalentTo(new[] { 1, 2, 3, 4, 5 }));
    }
}
