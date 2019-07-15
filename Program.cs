using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ouvidoria.Repository;
using Ouvidoria.Business;
using Ouvidoria.Models;
using ECV.Util;
using NHibernate;
using System.Net.Mail;
using System.IO;
using NReco.ImageGenerator;
using System.IO.Ports;

namespace RHEnvioEmail
{
    static class Program
    {
        
        [STAThread]
        static void Main()
        {
            SerialPort myPort = new SerialPort();
            myPort.BaudRate = 9600;
            myPort.PortName = "COM4";
            myPort.Open();

            while (true)
            {
                string data_rx = myPort.ReadLine();

                var tag = data_rx.Substring(0, 22).Replace(" ", "");
                var turma = data_rx.Substring(24, 3);
                Aluno aluno = AlunoRepository.Instance.GetFirst(new Aluno { Pessoa = new Pessoa { Tag = tag } },true);

                if (aluno != null)
                {
                    Aula aula = new Aula { DataEHoraInicio = DateTime.Now, DataEHoraFinal = DateTime.Now, Turma = new Turma { Descricao = turma } };

                    aula = AulaRepository.Instance.GetListDay(aula).FirstOrDefault();


                    Frequencia frequencia = new Frequencia();

                    if (aluno.Turma.Descricao == turma)
                        frequencia = new Frequencia { Aluno = aluno, Aula = aula, alunoDaSala = true, DataEHoraCaptura = DateTime.Now };
                    else
                        frequencia = new Frequencia { Aluno = aluno, Aula = aula, alunoDaSala = false, DataEHoraCaptura = DateTime.Now };



                    FrequenciaRepository.Instance.SaveOrUpdateNoAudit(frequencia);
                }
            }

        }
    }
}
