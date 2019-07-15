using ECV.Util;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ouvidoria.Models
{
    [Serializable]
    public class FrequenciaConfirmada
    {
        public virtual Guid? Id { get; set; }

        public virtual Aluno Aluno { get; set; }

        public virtual Aula Aula { get; set; }

        public virtual bool? Presenca { get; set; }

    }
    public class FrequenciaConfirmadaMap : ClassMap<FrequenciaConfirmada>
    {
        public FrequenciaConfirmadaMap()
        {
            Id(x => x.Id).GeneratedBy.GuidNative();
            Map(x => x.Presenca);
            References<Aluno>(x => x.Aluno).Not.Nullable().Not.LazyLoad();
            References<Aula>(x => x.Aula).Not.Nullable().Not.LazyLoad();
        }
    }
}