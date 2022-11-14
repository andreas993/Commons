using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Common.WPF.UserControls
{
  public partial class Card : UserControl
  {
    public Card()
    {
      InitializeComponent();
    }

    public string Text
    {
      get { return (string)GetValue(TextProperty); }
      set { SetValue(TextProperty, value); }
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(Card));


    public string Value
    {
      get { return (string)GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(string), typeof(Card));



    public ImageSource Source
    {
      get { return (ImageSource)GetValue(SourceProperty); }
      set { SetValue(SourceProperty, value); }
    }

    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register("Source", typeof(ImageSource), typeof(Card));


  }
}
