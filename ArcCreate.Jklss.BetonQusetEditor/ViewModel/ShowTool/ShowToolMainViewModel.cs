using ArcCreate.Jklss.BetonQusetEditor.Base;
using ArcCreate.Jklss.BetonQusetEditor.View.ShowTool;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace ArcCreate.Jklss.BetonQusetEditor.ViewModel.ShowTool
{
    public partial class ShowToolMainViewModel: ObservableObject
    {
        public ShowToolMainViewModel(ShowToolMainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        private ShowToolMainWindow mainWindow;

        private bool isShow = false;

        private bool isMove = false;

        private bool isDoing = false;

        private Button btn;

        [RelayCommand()]
        private void Loaded(Button button)
        {
            this.btn = button;
            button.Click += Button_Click;
        }

        [RelayCommand()]
        private void GridLoaded(Grid grid)
        {
            grid.MouseMove += Grid_MouseMove;
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                btn.IsEnabled = false;

                mainWindow.DragMove();

                isMove = true;
            }
            else
            {
                btn.IsEnabled = true;
            }
        }

        private async void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (isMove)
            {
                isMove = !isMove;

                return;
            }

            if (isDoing)
            {
                return;
            }

            if (!isShow)
            {
                await Task.Run(() =>
                {
                    isDoing = true;
                    for (int i = 0; i < 8; i++)
                    {
                        if (i == 0)
                        {
                            mainWindow.Dispatcher.Invoke(new Action(() =>
                            {
                                AnimationBase.Appear(mainWindow.Grid_One, 3, 10, 0.1);
                            }));
                            
                        }
                        else if (i == 1)
                        {
                            mainWindow.Dispatcher.Invoke(new Action(() =>
                            {
                                AnimationBase.Appear(mainWindow.Grid_Two, 3, 10, 0.1);
                            }));
                            
                        }
                        else if (i == 2)
                        {
                            mainWindow.Dispatcher.Invoke(new Action(() =>
                            {
                                AnimationBase.Appear(mainWindow.Grid_Three, 2, 10, 0.1);
                            }));
                            
                        }
                        else if (i == 3)
                        {
                            mainWindow.Dispatcher.Invoke(new Action(() =>
                            {
                                AnimationBase.Appear(mainWindow.Grid_Four, 2, 10, 0.1);
                            }));
                            
                        }
                        else if (i == 4)
                        {
                            mainWindow.Dispatcher.Invoke(new Action(() =>
                            {
                                AnimationBase.Appear(mainWindow.Grid_Five, 2, 10, 0.1);
                            }));
                            
                        }
                        else if (i == 5)
                        {
                            mainWindow.Dispatcher.Invoke(new Action(() =>
                            {
                                AnimationBase.Appear(mainWindow.Grid_Six, 1, 10, 0.1);
                            }));
                            
                        }
                        else if (i == 6)
                        {
                            mainWindow.Dispatcher.Invoke(new Action(() =>
                            {
                                AnimationBase.Appear(mainWindow.Grid_Seven, 0, 10, 0.1);
                            }));
                            
                        }
                        else
                        {
                            mainWindow.Dispatcher.Invoke(new Action(() =>
                            {
                                AnimationBase.Appear(mainWindow.Grid_Eight, 0, 10, 0.1);
                            }));
                            
                        }
                        Thread.Sleep(100);

                        
                    }

                    isShow = true;
                    isDoing = false;


                });
            }
            else
            {
                await Task.Run(() =>
                {
                    isDoing = true;
                    for (int i = 0; i < 8; i++)
                    {
                        if (i == 0)
                        {
                            mainWindow.Dispatcher.Invoke(new Action(() =>
                            {
                                AnimationBase.Disappear(mainWindow.Grid_One, 3, 10, 0.1);
                            }));
                            
                        }
                        else if (i == 1)
                        {
                            mainWindow.Dispatcher.Invoke(new Action(() =>
                            {
                                AnimationBase.Disappear(mainWindow.Grid_Two, 3, 10, 0.1);
                            }));
                            
                        }
                        else if (i == 2)
                        {
                            mainWindow.Dispatcher.Invoke(new Action(() =>
                            {
                                AnimationBase.Disappear(mainWindow.Grid_Three, 2, 10, 0.1);
                            }));
                            
                        }
                        else if (i == 3)
                        {
                            mainWindow.Dispatcher.Invoke(new Action(() =>
                            {
                                AnimationBase.Disappear(mainWindow.Grid_Four, 2, 10, 0.1);
                            }));
                            
                        }
                        else if (i == 4)
                        {
                            mainWindow.Dispatcher.Invoke(new Action(() =>
                            {
                                AnimationBase.Disappear(mainWindow.Grid_Five, 2, 10, 0.1);
                            }));
                            
                        }
                        else if (i == 5)
                        {
                            mainWindow.Dispatcher.Invoke(new Action(() =>
                            {
                                AnimationBase.Disappear(mainWindow.Grid_Six, 1, 10, 0.1);
                            }));
                            
                        }
                        else if (i == 6)
                        {
                            mainWindow.Dispatcher.Invoke(new Action(() =>
                            {
                                AnimationBase.Disappear(mainWindow.Grid_Seven, 0, 10, 0.1);
                            }));
                            
                        }
                        else
                        {
                            mainWindow.Dispatcher.Invoke(new Action(() =>
                            {
                                AnimationBase.Disappear(mainWindow.Grid_Eight, 0, 10, 0.1);
                            }));
                            
                        }
                        Thread.Sleep(100);
                    }
                    isShow = false;
                    isDoing = false;
                });
            }
        }

        [RelayCommand()]
        private void MainClick()
        {
            Environment.Exit(0);
        }

        [RelayCommand()]
        private void BetonQuestOpen()
        {
            MainWindow window = new MainWindow();
            window.Show();

            mainWindow.Close();
        }
    }
}
