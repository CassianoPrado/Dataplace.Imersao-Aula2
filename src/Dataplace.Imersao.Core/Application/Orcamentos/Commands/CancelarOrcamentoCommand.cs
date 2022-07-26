using Dataplace.Core.Domain.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataplace.Imersao.Core.Application.Orcamentos.Commands
{
    public class CancelarOrcamentoCommand : Command
    {
        public int NumOcamento { get; set; }
    }

}
