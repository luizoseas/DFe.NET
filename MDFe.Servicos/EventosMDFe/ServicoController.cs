﻿/********************************************************************************/
/* Projeto: Biblioteca ZeusMDFe                                                 */
/* Biblioteca C# para emissão de Manifesto Eletrônico Fiscal de Documentos      */
/* (https://mdfe-portal.sefaz.rs.gov.br/                                        */
/*                                                                              */
/* Direitos Autorais Reservados (c) 2014 Adenilton Batista da Silva             */
/*                                       Zeusdev Tecnologia LTDA ME             */
/*                                                                              */
/*  Você pode obter a última versão desse arquivo no GitHub                     */
/* localizado em https://github.com/adeniltonbs/Zeus.Net.NFe.NFCe               */
/*                                                                              */
/*                                                                              */
/*  Esta biblioteca é software livre; você pode redistribuí-la e/ou modificá-la */
/* sob os termos da Licença Pública Geral Menor do GNU conforme publicada pela  */
/* Free Software Foundation; tanto a versão 2.1 da Licença, ou (a seu critério) */
/* qualquer versão posterior.                                                   */
/*                                                                              */
/*  Esta biblioteca é distribuída na expectativa de que seja útil, porém, SEM   */
/* NENHUMA GARANTIA; nem mesmo a garantia implícita de COMERCIABILIDADE OU      */
/* ADEQUAÇÃO A UMA FINALIDADE ESPECÍFICA. Consulte a Licença Pública Geral Menor*/
/* do GNU para mais detalhes. (Arquivo LICENÇA.TXT ou LICENSE.TXT)              */
/*                                                                              */
/*  Você deve ter recebido uma cópia da Licença Pública Geral Menor do GNU junto*/
/* com esta biblioteca; se não, escreva para a Free Software Foundation, Inc.,  */
/* no endereço 59 Temple Street, Suite 330, Boston, MA 02111-1307 USA.          */
/* Você também pode obter uma copia da licença em:                              */
/* http://www.opensource.org/licenses/lgpl-license.php                          */
/*                                                                              */
/* Zeusdev Tecnologia LTDA ME - adenilton@zeusautomacao.com.br                  */
/* http://www.zeusautomacao.com.br/                                             */
/* Rua Comendador Francisco josé da Cunha, 111 - Itabaiana - SE - 49500-000     */
/********************************************************************************/

using MDFe.Classes.Extencoes;
using MDFe.Classes.Informacoes.Evento;
using MDFe.Classes.Informacoes.Evento.Flags;
using MDFe.Classes.Retorno.MDFeEvento;
using MDFe.Servicos.EventosMDFe.Contratos;
using MDFe.Servicos.Factory;
using MDFeEletronico = MDFe.Classes.Informacoes.MDFe;

namespace MDFe.Servicos.EventosMDFe
{
    public class ServicoController : IServicoController
    {
        public MDFeRetEventoMDFe Executar(MDFeEletronico mdfe, byte sequenciaEvento, MDFeEventoContainer eventoContainer, MDFeTipoEvento tipoEvento)
        {
            var evento = FactoryEvento.CriaEvento(mdfe,
                tipoEvento,
                sequenciaEvento,
                eventoContainer);


            string chave = evento.InfEvento.Id;

            if (mdfe != null)
            {
                chave = mdfe.Chave();
            }

            evento.ValidarSchema();
            evento.SalvarXmlEmDisco(chave);

            var webService = WsdlFactory.CriaWsdlMDFeRecepcaoEvento();
            var retornoXml = webService.mdfeRecepcaoEvento(evento.CriaXmlRequestWs());

            var retorno = MDFeRetEventoMDFe.LoadXml(retornoXml.OuterXml, evento);
            retorno.SalvarXmlEmDisco(chave);

            return retorno;
        }
    }
}