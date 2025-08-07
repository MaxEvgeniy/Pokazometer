using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeatDriver
{
    public partial class Form1 : Form
    {
        int WheatherTimerCount;        // Счетчик моделирования температуры окружающей среды
        int WheatherTimerStep = 500;   // Длительность шага в мкС счетчика моделирования температуры окружающей среды
        double TAmbientCurrent = 0;    // Текущая температура окружающей среды
        double TProbeCurrent = 0;      // Текущая температура пробы

        double TProbeOLD = 0;          // Температура пробы предыдущего шага
        double DTProbe = 0;            // Разность текущей и предыдущей температур пробы

        double KPD = 0.8;              // КПД нагревателя
        double LosePower = 10;         // Мощность потерь тепла в ваттах
        double MCLoseCoef = 500;        // Коэффициент влияния теплопотерь

        double MCCoef = 200;           // Коэффициент, заменяющий произведение массы на удельную темплоемкость
        Boolean HeaterONNow = false;   // Мгновенное состояние нагревателя на текущий момент
        Boolean HeaterONOld = false;   // Мгновенное состояние нагревателя на предыдущий шаг

        uint HeaterPower = 500;        // Мощность нагревателя в ваттах
        double THeater = 0.0001;       // Температура поверхности нагревателя
        double THeaterOLD = 0.0001;    // Температура поверхности нагревателя от предыдущего шага расчета
        int MinHeaterTime = 10;        // Минимальное время работы нагревателя в мкС или время работы 1 шага
        double MinQHeater = 0;         // Количество энергии, выделенное на нагревателе за 1 шаг работы
        double MCHeaterCoef = 200;     // Коэффициент, заменяющий произведение массы на удельную темплоемкость нагревателя

        ThermalArea Sphere;
        ThermalArea ClosedWater;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            TAmbientCurrent = 25.0;                     // Стартовая температура окружающей среды
            WheatherTimer.Interval = WheatherTimerStep;
            HeaterTimer.Interval = MinHeaterTime;
            THeater = TAmbientCurrent;
            THeaterOLD = TAmbientCurrent;
            ///////////////////////////////////////////////////////////////////////////////////////////////////////


            ///////////////////////////////////////////////////////////////////////////////////////////////////////
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (WheatherTimer.Enabled == false)
            {
                WheatherTimer.Start();
                HeaterTimer.Start();
            }
            else
            {
                WheatherTimer.Stop();
                HeaterTimer.Stop();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            WheatherTimerCount = WheatherTimerCount + WheatherTimerStep / 1000;
            if (WheatherTimerCount > 1000) { WheatherTimerCount = 0; }
            label1.Text = Convert.ToString(WheatherTimerCount);

            //if (HeaterONNow == true)
            //{
            //    if (HeaterONOld == true)
            //    {
            //        TProbeCurrent = TProbeCurrent + ((HeaterPower * WheatherTimerStep) / (MCCoef * 1000));
            //    }
            //    else
            //    {
            //        HeaterONOld = true;
            //    }
            //}
            //else
            //{
            //    if (HeaterONOld == true)
            //    {
            //        HeaterONOld = false;
            //    }
            //    else
            //    {

            //    }
            //}
            //LosePower = MCCoef * (TProbeCurrent - TAmbientCurrent);
            //TProbeCurrent = TProbeCurrent - ((LosePower * WheatherTimerStep) / (LoseCoef*1000));

            //chart1.Series[0].Points.AddXY(WheatherTimerCount, TAmbientCurrent);
            //chart1.Series[1].Points.AddXY(WheatherTimerCount, TProbeCurrent);

            chart1.Series[2].Points.AddXY(WheatherTimerCount, THeater);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (HeaterONNow == false)
            {
                HeaterONNow = true;
                button2.Text = "Heater On";
            }
            else
            {
                HeaterONNow = false;
                button2.Text = "Heater Off";
            }
        }

        private void CalculateTimer_Tick(object sender, EventArgs e)
        {

        }

        private void HeaterTimer_Tick(object sender, EventArgs e)
        {
            if (HeaterONNow == true)
            {
                //THeater = THeaterOLD + (HeaterPower * MinHeaterTime / 1000 * MCHeaterCoef); // Температура нагревателя без потерь.

                THeater = THeaterOLD + (HeaterPower * MinHeaterTime / 1000 * (MCHeaterCoef - LosePower));

                THeaterOLD = THeater;
            }

        }
    }

    public class ThermalArea
    {
        double tArea;                 // Масса вещества области, кг
        double tHeatCapacity;         // Теплоемкость вещества области, Дж/(кг*К)
        double tThermalConductivity;  // Теплопроводность области, Вт/(м^2*К)
        double tStartTemperature;     // Температура области до начала эксперимента, К)
        public ThermalArea()
        {
        }
    }
    public class ContactArea
    {
        ThermalArea aArea1;          // Тепловая зона 1
        ThermalArea aArea2;          // Тепловая зона 2
        double aContactSquare;       // Площадь контакта, мм^2)
        double aResistance;          // Тепловое сопротивление контакта, мм^2)
        public ContactArea()
        {
        }
    } // Тепловой контакт 2-х зон
}
