using System.Collections.Generic;
using System.IO.Abstractions;
using System.Text;
using System.Threading.Tasks;
using ClassicBasic.Interpreter;
using ClassicBasic.Interpreter.Exceptions;
using SadConsole;
using SadConsole.Components;
using SadConsole.Input;
using SadRogue.Primitives;

namespace BasicTerminal
{
    class ConsoleBASICInterpreter : SadConsole.Components.IComponent, ITeletype, ITeletypeWithPosition
    {
        private Console _console;

        public IRunEnvironment BASICRunEnvironment;
        public IProgramRepository BASICProgramRepository;
        public ITokensProvider BASICTokensProvider;
        public ITokeniser BASICTokeniser;
        public ExecutorFrameBased BASICExecutor;
        public IVariableRepository BASICVariableRepository;
        public IDataStatementReader BASICDataStatement;
        public IExpressionEvaluator BASICExpresionEval;
        public IFileSystem BASICFileSystem;

        private Task RunProgramTask;
        
        public ConsoleBASICInterpreter()
        {
            BASICRunEnvironment = new RunEnvironment();
            BASICProgramRepository = new ProgramRepository();
            BASICVariableRepository = new VariableRepository();
            BASICDataStatement = new DataStatementReader(BASICRunEnvironment, BASICProgramRepository);
            BASICExpresionEval = new ExpressionEvaluator(BASICVariableRepository, BASICRunEnvironment);
            BASICFileSystem = new FileSystem();

            BASICTokensProvider = new TokensProvider(new IToken[] {
                new ClassicBasic.Interpreter.Commands.Clear(BASICRunEnvironment, BASICVariableRepository, BASICDataStatement),
                new ClassicBasic.Interpreter.Commands.Cont(BASICRunEnvironment,BASICProgramRepository),
                new ClassicBasic.Interpreter.Commands.Data(BASICRunEnvironment),
                new ClassicBasic.Interpreter.Commands.Def(BASICRunEnvironment,BASICExpresionEval),
                new ClassicBasic.Interpreter.Commands.Del(BASICRunEnvironment,BASICProgramRepository),
                new ClassicBasic.Interpreter.Commands.Dim(BASICRunEnvironment, BASICExpresionEval, BASICVariableRepository),
                new ClassicBasic.Interpreter.Commands.Edit(BASICRunEnvironment,BASICProgramRepository, this),
                new ClassicBasic.Interpreter.Commands.Else(BASICRunEnvironment),
                new ClassicBasic.Interpreter.Commands.End(BASICRunEnvironment),
                new ClassicBasic.Interpreter.Commands.For(BASICRunEnvironment,BASICExpresionEval, BASICVariableRepository),
                new ClassicBasic.Interpreter.Commands.Get(BASICRunEnvironment,BASICExpresionEval,this),
                new ClassicBasic.Interpreter.Commands.Gosub(BASICRunEnvironment,BASICProgramRepository),
                new ClassicBasic.Interpreter.Commands.Goto(BASICRunEnvironment,BASICExpresionEval, BASICProgramRepository),
                new ClassicBasic.Interpreter.Commands.If(BASICRunEnvironment,BASICExpresionEval, BASICProgramRepository),
                new ClassicBasic.Interpreter.Commands.Input(BASICRunEnvironment,BASICExpresionEval,BASICVariableRepository, this),
                new ClassicBasic.Interpreter.Commands.Let(BASICRunEnvironment,BASICExpresionEval),
                new ClassicBasic.Interpreter.Commands.List(BASICProgramRepository,this, BASICRunEnvironment),
                new ClassicBasic.Interpreter.Commands.Load(BASICExpresionEval, BASICFileSystem, BASICProgramRepository),
                new ClassicBasic.Interpreter.Commands.New(BASICRunEnvironment,BASICProgramRepository,BASICVariableRepository,BASICDataStatement),
                new ClassicBasic.Interpreter.Commands.Next(BASICRunEnvironment,BASICVariableRepository),
                new ClassicBasic.Interpreter.Commands.On(BASICRunEnvironment,BASICExpresionEval,BASICProgramRepository),
                new ClassicBasic.Interpreter.Commands.OnErr(BASICRunEnvironment),
                new ClassicBasic.Interpreter.Commands.Pop(BASICRunEnvironment),
                new ClassicBasic.Interpreter.Commands.Print(BASICRunEnvironment, BASICExpresionEval, this),
                new ClassicBasic.Interpreter.Commands.Read(BASICRunEnvironment,BASICExpresionEval,BASICDataStatement),
                new ClassicBasic.Interpreter.Commands.Remark(BASICRunEnvironment),
                new ClassicBasic.Interpreter.Commands.Restore(BASICRunEnvironment,BASICDataStatement),
                new ClassicBasic.Interpreter.Commands.Resume(BASICRunEnvironment,BASICProgramRepository),
                new ClassicBasic.Interpreter.Commands.Return(BASICRunEnvironment),
                new ClassicBasic.Interpreter.Commands.Run(BASICRunEnvironment,BASICProgramRepository, BASICVariableRepository,BASICDataStatement),
                //new ClassicBasic.Interpreter.Commands.Save(BASICRunEnvironment,BASICProgramRepository),
                new ClassicBasic.Interpreter.Commands.Stop(BASICRunEnvironment),
            });
            BASICTokeniser = new Tokeniser(BASICTokensProvider);
            BASICExecutor = new ExecutorFrameBased(this, BASICRunEnvironment, BASICProgramRepository, BASICTokensProvider, BASICTokeniser);
        }

        public uint SortOrder { get; set; }

        public bool IsUpdate => true;

        public bool IsRender => false;

        public bool IsMouse => false;

        public bool IsKeyboard => true;


        public void OnAdded(IScreenObject host)
        {
            if (!(host is Console)) throw new System.ArgumentException("Component can only be added to a console");
            _console = (Console)host;

            _console.Surface.DefaultGlyph = ' ';
            _console.Surface.DefaultBackground = Color.Black;
            _console.Surface.DefaultForeground = Color.White;
            _console.Clear();

            _console.Cursor.Move(0, 0).Print("READY.").NewLine();
        }

        public void OnRemoved(IScreenObject host) { }

        bool isInput = false;

        public void ProcessKeyboard(IScreenObject host, Keyboard keyboard, out bool handled)
        {
            handled = false;

            // Program is running -- Use special input handlers.
            if (BASICExecutor.IsExecuting)
            {
                // Break program
                if (keyboard.IsKeyDown(Keys.C) && (keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.RightControl)))
                {
                    BASICExecutor.Break();
                }

                handled = true;
                return;
            }

            // No program is running, in editor mode.
            if (keyboard.IsKeyPressed(Keys.Enter))
            {
                var command = Read();
                if (command != null)
                {
                    RunProgramTask = new Task(() =>
                    {

                        try
                        {
                            var parsedLine = BASICTokeniser.Tokenise(command.Trim());
                            if (parsedLine.LineNumber.HasValue)
                            {
                                BASICProgramRepository.SetProgramLine(parsedLine);
                                NewLine();
                            }
                            else
                            {
                                BASICRunEnvironment.CurrentLine = parsedLine;
                                NewLine();
                                bool quit = BASICExecutor.ExecuteLine();

                                if (!BASICExecutor.IsExecuting)
                                    WritePrompt();
                            }
                        }
                        catch (BreakException endError)
                        {
                            if (endError.ErrorMessage != string.Empty)
                            {
                                WriteErrorToTeletype(
                                    BASICRunEnvironment.CurrentLine.LineNumber,
                                    endError.ErrorMessage);
                            }
                        }
                        catch (BasicException basicError)
                        {
                            WriteErrorToTeletype(
                                BASICRunEnvironment.DataErrorLine ?? BASICRunEnvironment.CurrentLine?.LineNumber,
                                "?" + basicError.ErrorMessage + " ERROR");
                        }
                    });
                    RunProgramTask.Start();
                }

                handled = true;
            }

        }

        private void WriteErrorToTeletype(int? lineNumber, string message)
        {
            _console.Cursor.NewLine()
                           .Print(message + (lineNumber.HasValue ? $" IN {lineNumber}" : string.Empty))
                           .NewLine();
            WritePrompt();
        }

        private void WritePrompt() =>
                _console.Cursor.NewLine().Print("READY.").NewLine();

        public void Update(IScreenObject host, System.TimeSpan delta)
        {
            if (BASICExecutor.IsExecuting)
            {
                bool quit = BASICExecutor.ExecuteLine();

                if (!BASICExecutor.IsExecuting)
                    WritePrompt();
            }

        }

        public void ProcessMouse(IScreenObject host, MouseScreenObjectState state, out bool handled) =>
            throw new System.NotImplementedException();

        
        public void Render(IScreenObject host, System.TimeSpan delta) { }





        private bool _canEdit;
        private string _editText;

        public short Width => (short)_console.Width;

        public bool CanEdit => _canEdit;

        public string EditText { set => _editText = value; }

        public event System.ConsoleCancelEventHandler CancelEventHandler;


        public string Read()
        {
            return _console.GetString(Point.ToIndex(0, _console.Cursor.Position.Y, _console.Width), _console.Width);
        }

        public char ReadChar()
        {
            throw new System.NotImplementedException();
        }

        public void Write(string output) =>
            _console.Cursor.Print(output);

        public void Tab(short count) =>
            _console.Cursor.Print(new string(' ', count * 4));

        public void Space(short count) =>
            _console.Cursor.Print(new string(' ', count));

        public void NextComma() =>
            _console.Cursor.Print(",");

        public void NewLine() =>
            _console.Cursor.NewLine();

        public short Position() =>
            (short)(_console.Cursor.Position.X + 1);

        public void ClearScreen()
        {
            _console.Clear();
            _console.Cursor.Position = Point.Zero;
        }
            
    }
}
