using Dataplace.Core.Infra.Data.OdbcConnection;
using Dataplace.Imersao.Core.Domain.Orcamentos;
using Dataplace.Imersao.Core.Domain.Orcamentos.Repositories;
using dpLibrary05;
using Dapper;
using Dataplace.Imersao.Core.Domain.Orcamentos.Enums;
using Dataplace.Imersao.Core.Domain.Orcamentos.ValueObjects;
using System.Linq;
using System.Collections.Generic;

namespace Dataplace.Imersao.Core.Infra.Data.Repositories
{
    public class OrcamentoItemRepository : IOrcamentoItemRepository
    {
        #region feilds
        private readonly IDataAccess _dataAccess = null;
        #endregion
        #region constructors
        public OrcamentoItemRepository(): this(BootStrapper.Container.GetInstance<IOdbcDataAccess>())
        {

        }
        public OrcamentoItemRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        #endregion
        public OrcamentoItem AdicionarItem(OrcamentoItem entity)
        {
            //var sql = @"
            //INSERT INTO 
            //   OrcamentoItem(CdEmpresa, CdFilial, NumOrcamento, qtdproduto, stitem, tpregistro, cdproduto, vlvenda, percaltpreco, vlcalculado)
            //VALUES(?,?,?,?,?,?,?,?,?,?)

            //SELECT Seq, CdEmpresa, CdFilial, NumOrcamento, qtdproduto, stitem, tpregistro as TpProduto, cdproduto, vlvenda as PrecoTabela, vlcalculado as PrecoVenda, percaltpreco as PercAltPreco
            //    FROM  
            //OrcamentoItem
            //    WHERE seq = (SELECT SCOPE_IDENTITY())
            //";
            //var items = _dataAccess.Connection.Query<OrcamentoItem, OrcamentoProduto, OrcamentoItemPreco, OrcamentoItem>(sql,
            //    (item, produto, preco) => {

            //        if (produto != null)
            //            item.DefinirProduto(produto);

            //        if (preco != null)
            //            item.DefinirPreco(preco);

            //        return item;
            //    },
            //    new
            //    {
            //        CdEmpresa = entity.CdEmpresa,
            //        CdFilial = entity.CdFilial,
            //        NumOrcamento = entity.NumOrcamento,
            //        qtdproduto = entity.Quantidade,
            //        stitem = entity.Situacao.ToDataValue(),
            //        tpregistro = entity.Produto.TpProduto.ToDataValue(),
            //        cdproduto = entity.Produto.CdProduto,
            //        vlvenda = entity.Preco?.PrecoTabela,
            //        percaltpreco = entity.Preco?.PercAltPreco,
            //        vlcalculado = entity.Preco?.PrecoVenda
            //    },
            //     splitOn : "TpProduto, PrecoTabela",
            //     transaction: _dataAccess.Transaction); 
            //return items.FirstOrDefault();










            var sql = @"
            INSERT INTO 
               OrcamentoItem(CdEmpresa, CdFilial, NumOrcamento, qtdproduto, stitem, tpregistro, cdproduto, vlvenda, percaltpreco, vlcalculado)
            VALUES(?,?,?,?,?,?,?,?,?,?);

            SELECT SCOPE_IDENTITY()
            ";
            var result = _dataAccess.Connection.ExecuteScalar<int?>(sql,
                new
                {
                    CdEmpresa = entity.CdEmpresa,
                    CdFilial = entity.CdFilial,
                    NumOrcamento = entity.NumOrcamento,
                    qtdproduto = entity.Quantidade,
                    stitem = entity.Situacao.ToDataValue(),
                    tpregistro = entity.Produto.TpProduto.ToDataValue(),
                    cdproduto = entity.Produto.CdProduto,
                    vlvenda = entity.Preco?.PrecoTabela,
                    percaltpreco = entity.Preco?.PercAltPreco,
                    vlcalculado = entity.Preco?.PrecoVenda
                },
                 transaction: _dataAccess.Transaction);

            return result.HasValue ? ObterItem(entity.CdEmpresa, entity.CdFilial , entity.NumOrcamento, result.Value) : default;
           
        }

        public bool AtualizarItem(OrcamentoItem entity)
        {
            var sql = @"
            UPDATE 
                Orcamento
            SET
                qtdproduto= ?, stitem= ?, tpregistro= ?, cdproduto= ?, vlvenda= ?, percaltpreco= ?, vlcalculado= ?
            WHERE CdEmpresa = ?
                AND CdFilial = ?
                AND NumOrcamento = ?
                AND Seq = ?
            ";
           return  _dataAccess.Connection.Execute(sql, 
               new {

                   qtdproduto = entity.Quantidade,
                   stitem = entity.Situacao.ToDataValue(),
                   tpregistro = entity.Produto.TpProduto.ToDataValue(),
                   cdproduto = entity.Produto.CdProduto,
                   vlvenda = entity.Preco?.PrecoTabela,
                   percaltpreco = entity.Preco?.PercAltPreco,
                   vlcalculado = entity.Preco?.PrecoVenda,
                   CdEmpresa = entity.CdEmpresa,
                   CdFilial = entity.CdFilial,
                   NumOrcamento = entity.NumOrcamento,
                   Seq = entity.Seq,
               },
               transaction: _dataAccess.Transaction) > 0 ;
        }

        public bool ExcluirItem(OrcamentoItem entity)
        {
            var sql = @"
            DELETE FROM OrcamentoItem
            WHERE CdEmpresa = ?
                AND CdFilial = ?
                AND NumOrcamento = ?
                AND Seq = ?
            ";

            return _dataAccess.Connection.Execute(sql,
                new
                {
                    CdEmpresa = entity.CdEmpresa,
                    CdFilial = entity.CdFilial,
                    NumOrcamento = entity.NumOrcamento,
                    Seq = entity.Seq
                },
                transaction: _dataAccess.Transaction) > 0;
        }

        public OrcamentoItem ObterItem(string cdEmpresa, string cdFilial, int numOrcamento, int seq)
        {
            var sql = @"
            SELECT Seq, CdEmpresa, CdFilial, NumOrcamento, qtdproduto, stitem, tpregistro as TpProduto, cdproduto, vlvenda as PrecoTabela, vlcalculado as PrecoVenda, percaltpreco as PercAltPreco
                FROM  
            OrcamentoItem
                WHERE CdEmpresa = ? AND CdFilial = ? AND NumOrcamento = ? AND Seq = ?
            ";
            var items = _dataAccess.Connection.Query<OrcamentoItem, OrcamentoProduto, OrcamentoItemPreco, OrcamentoItem>(sql,
                (item, produto, preco) => {

                    if (produto != null)
                        item.DefinirProduto(produto);

                    if (preco != null)
                        item.DefinirPreco(preco);

                    return item;
                },
                new
                {
                    CdEmpresa = cdEmpresa,
                    CdFilial = cdFilial,
                    NumOrcamento = numOrcamento,
                    Seq = seq
                },
                splitOn: "TpProduto, PrecoTabela",
                transaction: _dataAccess.Transaction);

            return items.FirstOrDefault();
        }

        public IEnumerable<OrcamentoItem> ObterItens(string cdEmpresa, string cdFilial, int numOrcamento)
        {
            var sql = @"
            SELECT Seq, CdEmpresa, CdFilial, NumOrcamento, qtdproduto, stitem, tpregistro as TpProduto, cdproduto, vlvenda as PrecoTabela, vlcalculado as PrecoVenda, percaltpreco as PercAltPreco
                FROM  
            OrcamentoItem
                WHERE CdEmpresa = ? AND CdFilial = ? AND NumOrcamento = ?
            ";
            var items = _dataAccess.Connection.Query<OrcamentoItem, OrcamentoProduto, OrcamentoItemPreco, OrcamentoItem>(sql,
                (item, produto, preco) => {

                    if (produto != null)
                        item.DefinirProduto(produto);

                    if (preco != null)
                        item.DefinirPreco(preco);

                    return item;
                },
                new
                {
                    CdEmpresa = cdEmpresa,
                    CdFilial = cdFilial,
                    NumOrcamento = numOrcamento
                },
                splitOn: "TpProduto, PrecoTabela",
                transaction: _dataAccess.Transaction);

            return items;
        }

    }
}
