using ECV.Util;
using Ouvidoria.Business;
using Ouvidoria.Models;
using Ouvidoria.Repository;
using Ouvidoria.Util;
using PagedList;
using Rotativa;
using Rotativa.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ouvidoria.Controllers
{
    public class FrequenciaController : Controller
    {
        [Authorize]
        public ActionResult Listar(int? page, Frequencia exemplo, List<RestricaoConsulta> restricoes)
        {
            return View(new Tuple<IPagedList<Frequencia>, Frequencia, List<RestricaoConsulta>>(FrequenciaRepository.Instance.GetPagedList(page, Constantes.QUANTIDADE_REGISTROS_PAGINA, exemplo, restricoes), exemplo, restricoes));
        }

        public ActionResult Adicionar()
        {
            List<SelectListItem> nullList = new List<SelectListItem>();
            nullList.Add(new SelectListItem() { Value = null, Text = null });
            var aulasAux = AulaRepository.Instance.GetDropDownList(true);
            foreach (var aulaAux in aulasAux)
            {
                if (!GenericFunctions.IsNull(aulaAux))
                    nullList.Add(new SelectListItem() { Value = aulaAux.Id.ToString(), Text = aulaAux.DataEHoraInicio.Value.ToString() + " a " + aulaAux.DataEHoraFinal.Value.ToString() });
            }
            ViewBag.aulas = new SelectList(nullList, "Value", "Text");
            return View();
        }

        public ActionResult Alterar(Guid frequencia)
        {
            List<SelectListItem> nullList = new List<SelectListItem>();
            nullList.Add(new SelectListItem() { Value = null, Text = null });
            var aulasAux = AulaRepository.Instance.GetDropDownList(true);
            foreach (var aulaAux in aulasAux)
            {
                if (!GenericFunctions.IsNull(aulaAux))
                    nullList.Add(new SelectListItem() { Value = aulaAux.Id.ToString(), Text = String.Format("{0:dd/MM/yyyy HH:mm}", aulaAux.DataEHoraInicio) + " a " + String.Format("{0:dd/MM/yyyy HH:mm}", aulaAux.DataEHoraFinal)});
            }
            ViewBag.aulas = new SelectList(nullList, "Value", "Text");
                        
            Frequencia frequenciaRetorno = FrequenciaRepository.Instance.GetById(frequencia);
            return View(frequenciaRetorno);
        }

        [HttpPost]
        public JsonResult Salvar(Frequencia frequencia)
        {
            if (!BFrequencia.Instance.ValidarSalvar(ref frequencia))
            {
                return Json(new { Status = BFrequencia.Instance.Status(), Message = BFrequencia.Instance.MessageSalvar() }, JsonRequestBehavior.AllowGet);
            }
            try
            {
                FrequenciaRepository.Instance.SaveOrUpdateNoAudit(frequencia);
                return Json(new { Status = 0, Message = BFrequencia.Instance.MessageSalvar() }, JsonRequestBehavior.AllowGet);
            }
            catch (NHibernate.StaleStateException dbcx)
            {
                return Json(new { Status = Constantes.STATUS_ERRO_GENERICO, Message = ECV.Util.Mensagens.ERRO_CONCORRENCIA }, JsonRequestBehavior.AllowGet);
            }
            catch (RepositoryException ex)
            {
                return Json(new { Status = Constantes.STATUS_ERRO_GENERICO, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult SalvarFrequencias(List<FrequenciaConfirmada> frequencias)
        {

            try
            {
                FrequenciaConfirmadaRepository.Instance.SaveOrUpdateNoAudit(frequencias);
                return Json(new { Status = 0, Message = BFrequencia.Instance.MessageSalvar() }, JsonRequestBehavior.AllowGet);
            }
            catch (NHibernate.StaleStateException dbcx)
            {
                return Json(new { Status = Constantes.STATUS_ERRO_GENERICO, Message = ECV.Util.Mensagens.ERRO_CONCORRENCIA }, JsonRequestBehavior.AllowGet);
            }
            catch (RepositoryException ex)
            {
                return Json(new { Status = Constantes.STATUS_ERRO_GENERICO, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Buscar(Frequencia frequencia)
        {
            Frequencia frequenciaRetorno = FrequenciaRepository.Instance.GetFirst(frequencia);

            if (frequenciaRetorno != null)
            {
                return Json(new { Status = Constantes.STATUS_SUCESSO, Message = ECV.Util.Mensagens.ERRO_GENERICO }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Status = Constantes.STATUS_ERRO_GENERICO, Message = Ouvidoria.Util.Mensagens.PROTOCOLO_NAO_ENCONTRADO }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FrequenciaLogica(Guid id)
        {
            List<FrequenciaConfirmada> frequencias = BFrequencia.Instance.TratarFrequencias(id);
            return View(frequencias);
        }
    }
}