﻿using Dataplace.Core.Domain.Commands;
using Dataplace.Imersao.Core.Application.Orcamentos.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dataplace.Imersao.Core.Application.Orcamentos.Commands
{
    public class ExcluirOrcamentoItemCommand : DeleteCommand<OrcamentoItemViewModel>
    {
        public ExcluirOrcamentoItemCommand(OrcamentoItemViewModel items) : base(items)
        {
        }
    }

}
