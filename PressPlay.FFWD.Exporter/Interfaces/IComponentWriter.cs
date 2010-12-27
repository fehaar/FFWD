using System;
using System.Collections.Generic;
using System.Text;
using PressPlay.FFWD.Exporter.Writers;

namespace PressPlay.FFWD.Exporter.Interfaces
{
    public interface IComponentWriter
    {
        void Write(SceneWriter scene, object component);
    }

    public interface IOptionComponentWriter : IComponentWriter
    {
        string options { get; set; }
    }

    public interface IFilteredComponentWriter : IComponentWriter
    {
        Filter filter { get;  set; }
        Filter.FilterType defaultFilterType { get; }
    }
}
