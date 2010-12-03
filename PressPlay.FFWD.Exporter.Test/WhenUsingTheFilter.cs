using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Exporter.Test
{
    [TestFixture]
    public class WhenUsingTheFilter
    {
        Filter includeFilter;
        Filter excludeFilter;

        string item = "Item";

        [SetUp]
        public void SetUp( )
        {
            includeFilter = new Filter() { filterType = Filter.FilterType.Include, items = new[] { item } };
            excludeFilter = new Filter() { filterType = Filter.FilterType.Exclude, items = new[] { item } };
        }	

        [Test]
        public void ItemsWillBeIncludedIfTheyAreInTheListOfAnIncludeFilter()
        {
            Assert.That(includeFilter.Includes(item), Is.True);
        }

        [Test]
        public void ItemsWillBeNotIncludedIfTheyAreNotInTheListOfAnIncludeFilter()
        {
            Assert.That(includeFilter.Includes("Non item"), Is.False);
        }

        [Test]
        public void ItemsWillBeExcludedIfTheyAreInTheListOfAnExcludeFilter()
        {
            Assert.That(excludeFilter.Excludes(item), Is.True);
        }

        [Test]
        public void ItemsWillNotBeExcludedIfTheyAreNotInTheListOfAnExcludeFilter()
        {
            Assert.That(excludeFilter.Excludes("Non item"), Is.False);
        }

        [Test]
        public void ItemsWillBeIncludedIfItIsAnIncludeAllFilter()
        {
            includeFilter.filterType = Filter.FilterType.IncludeAll;
            Assert.That(includeFilter.Includes(item), Is.True);
            Assert.That(includeFilter.Includes("Non item"), Is.True);
        }

        [Test]
        public void ItemsWillNotBeExcludedIfItIsAnIncludeAllFilter()
        {
            includeFilter.filterType = Filter.FilterType.IncludeAll;
            Assert.That(includeFilter.Excludes(item), Is.False);
            Assert.That(includeFilter.Excludes("Non item"), Is.False);
        }

        [Test]
        public void ItemsWillNotBeIncludedIfItIsAnExcludeAllFilter()
        {
            includeFilter.filterType = Filter.FilterType.ExcludeAll;
            Assert.That(includeFilter.Includes(item), Is.False);
            Assert.That(includeFilter.Includes("Non item"), Is.False);
        }

        [Test]
        public void ItemsWillBeExcludedIfItIsAnExcludeAllFilter()
        {
            includeFilter.filterType = Filter.FilterType.ExcludeAll;
            Assert.That(includeFilter.Excludes(item), Is.True);
            Assert.That(includeFilter.Excludes("Non item"), Is.True);
        }

    }
}
