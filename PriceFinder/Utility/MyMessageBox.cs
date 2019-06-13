using BespokeFusion;
using PriceFinding.Styles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PriceFinding
{
   class MyMessageBox
   {
      /// <summary>
      /// Show message
      /// </summary>
      /// <param name="message">What you want to say</param>
      /// <param name="title">Title</param>
      /// <param name="canCancel">Is there a cancel button</param>
      /// <param name="canCopy">Is there a copy button</param>
      public static MessageBoxResult Show(string title, string message, bool canCancel = false, bool canCopy = false)
      {
         var msg = new CustomMaterialMessageBox
         {
            TxtMessage = {
               Text = message,
               Foreground = Brushes.Black
            },
            TxtTitle = {
               Text = title,
               Foreground = Brushes.White
            },
            BtnOk = {
               Content = "Ok" ,
               Background = (Brush)Application.Current.Resources["BrushPrimaryDark"],
               BorderBrush = (Brush)Application.Current.Resources["BrushPrimaryDark"]
            },

            MainContentControl = { Background = (Brush)Application.Current.Resources["BrushPrimaryLight"] },
            TitleBackgroundPanel = { Background = (Brush)Application.Current.Resources["BrushPrimaryDark"] },
            BorderBrush = (Brush)Application.Current.Resources["BrushPrimaryDark"]
         };

         if (!canCancel)
            msg.BtnCancel.Visibility = Visibility.Collapsed;

         if (!canCopy)
            msg.BtnCopyMessage.Visibility = Visibility.Collapsed;


         msg.Show();

         return msg.Result;
      }//Show

      //--------------------------------------------------------------------------------------------------//

      /// <summary>
      /// Show message without cancel button
      /// </summary>
      /// <param name="message">What you want to say</param>
      /// <param name="title">Title</param>
      /// <param name="canCopy">Is there a copy button</param>
      public static MessageBoxResult ShowOk(string title, string message, bool canCopy = true)
      {
         var msg = new CustomMaterialMessageBox
         {
            TxtMessage = {
               Text = message,
               Foreground = Brushes.Black
            },
            TxtTitle = {
               Text = title,
               Foreground = Brushes.White
            },
            BtnOk = {
               Content = "Ok" ,
               Background = (Brush)Application.Current.Resources["BrushPrimaryDark"],
               BorderBrush = (Brush)Application.Current.Resources["BrushPrimaryDark"]
            },

            ShowCloseButton = true,

            ResizeMode = ResizeMode.CanResizeWithGrip,

            MainContentControl = { Background = (Brush)Application.Current.Resources["BrushPrimaryLight"] },
            TitleBackgroundPanel = { Background = (Brush)Application.Current.Resources["BrushPrimaryDark"] },
            BorderBrush = (Brush)Application.Current.Resources["BrushPrimaryDark"]
         };


         msg.BtnCancel.Visibility = Visibility.Collapsed;

         if (!canCopy)
            msg.BtnCopyMessage.Visibility = Visibility.Collapsed;

         msg.Show();

         return msg.Result;
      }//Show


   }//Cls
}//NS
