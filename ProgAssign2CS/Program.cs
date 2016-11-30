using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgAssign2CS
{
    class Program
    {
        static int stackSize = 0;

        static void Main()
        {
            string filename;
            //Console.Write("Please specify a file name: ");
            //filename = Console.ReadLine();
            filename = "ProgrammingAssignment2SampleInput2.txt";

            Console.WriteLine(DateTime.Now.ToString("HH:MM:ss.ffffff"));
            var watch = System.Diagnostics.Stopwatch.StartNew();
            ProcessFile(filename);
            watch.Stop();

            Console.WriteLine(DateTime.Now.ToString("HH:MM:ss.ffffff"));
            Console.WriteLine("Elapsed time: " + (double)watch.ElapsedMilliseconds / 1000 + " seconds");

            double elpasedTime = (double)watch.ElapsedMilliseconds / 1000;
            for (int i = 0; i < 10; i++)
            {
                watch.Reset();
                watch.Start();
                ProcessFile(filename);
                watch.Stop();
                elpasedTime += (double)watch.ElapsedMilliseconds / 1000;
            }
            Console.WriteLine(DateTime.Now.ToString("HH:MM:ss.ffffff"));
            Console.WriteLine("Elapsed time: " + elpasedTime / 10 + " seconds");

            //RunTests();

            Console.WriteLine();
            Console.Write("Press any key to close this window...");
            Console.ReadKey();
        }

        static void TestStripe(string stripes, int expectedResult, ref int totalManualErrors, bool debug)
        {
            int result = Paint(stripes, debug);
            if (expectedResult != result)
            {
                if (debug)
                    Console.WriteLine("Result: " + result + ", Expected: " + expectedResult);
                totalManualErrors++;
            }

            if (debug)
                Console.WriteLine("=================================");
        }

        static void RunTests()
        {
            bool debug = false;
            int totalManualErrors = 0;

            //TestStripe("YBYB", 3, ref totalManualErrors, debug);
            //TestStripe("BGRGB", 3, ref totalManualErrors, debug);
            //TestStripe("YBYCYRY", 4, ref totalManualErrors, debug);
            //TestStripe("ABCD", 4, ref totalManualErrors, debug);
            //TestStripe("AAAA", 1, ref totalManualErrors, debug);
            //TestStripe("A", 1, ref totalManualErrors, debug);
            //TestStripe("", 0, ref totalManualErrors, debug);
            //TestStripe("AABBBCCCDABABBBB", 6, ref totalManualErrors, debug);

            //TestStripe("YBXCXBYYBBYM", 6, ref totalManualErrors, debug);
            //TestStripe("ABCACB", 4, ref totalManualErrors, debug);
            //TestStripe("JIZOZXJSOZXIJS", 10, ref totalManualErrors, debug);

            ////input1:
            //TestStripe("BECBBDDEEBABDCADEAAEABCACBDBEECDEDEACACCBEDABEDADD", 26, ref totalManualErrors, debug); //line 2
            //TestStripe("ETHRALAWUVRZICHDEAEQJUGXVFWCWE", 23, ref totalManualErrors, debug); //line 8
            //TestStripe("VYJICZOZZXJSTOZXGRPNIJSUH", 20, ref totalManualErrors, debug); //line 11
            //TestStripe("VLUGPXIOLYOGMVGYLXKYDOHLK", 20, ref totalManualErrors, debug); //line 17

            ////input2:
            //TestStripe("CBAKRXDCDXTQ", 10, ref totalManualErrors, debug); //line 731
            //TestStripe("YETLIOARQUSRLOURGLQWJFAGPUTS", 23, ref totalManualErrors, debug); //line 954
            //TestStripe("YGVVPYPPWGDDNNI", 9, ref totalManualErrors, debug); //line 1067
            //TestStripe("KARRVHQFHCGXBTHLHCKBJFVBKT", 19, ref totalManualErrors, debug); //line 2255
            //TestStripe("DLZJATSZIVICITVHAHIVKUJ", 17, ref totalManualErrors, debug); //line 5495

            //TestStripe("WDZJXKXZGRKFHNZOUFKNAD", 17, ref totalManualErrors, debug); //line 2296
            TestStripe("BGADTYTSDYEZIWDWKLDUWYXGGQLK", 21, ref totalManualErrors, debug); //line 5106
            TestStripe("DYEZIWDWKLDUWYXGGQLK", 13, ref totalManualErrors, debug); //line 5106
            
            //TestStripe("RZMAQJIBRNYHHGJOBDXVBZDTEXBPDZ", 24, ref totalManualErrors, debug); //line 6262



            if (totalManualErrors != 0)
            {
                Console.WriteLine("Total manual errors: " + totalManualErrors);
                Console.WriteLine("=================================");
            }
        }

        static int Paint(string stripes, bool debug = false)
        {
            int[] colorStartPos = new int[26] { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 };
            int[] colorEndPos = new int[26] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
            int[] widestColor = new int[26] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
            stackSize = 0;
            StringBuilder solutionLog = new StringBuilder();

            //Greedy choice:
            ChooseNextStripesGreedy(stripes, 0, stripes.Length - 1, colorStartPos, colorEndPos, widestColor);

            int minNumSweeps = 100000;
            int threadId = 1;
            for (int i = 0; i < 26 && widestColor[i] != -1; i++, threadId++)
            {
                if (widestColor[i] == -1)
                    break;

                StringBuilder localSolutionLog = new StringBuilder();
                char[] output = new string(Convert.ToChar(widestColor[i] + 65), stripes.Length).ToCharArray();
                int numSweeps = 1;
                if (debug)
                {
                    BuildSolutionLog(stripes.ToCharArray(), 0, localSolutionLog);
                    BuildSolutionLog(output, numSweeps, localSolutionLog);
                }

                int startPosToAnalyze = colorStartPos[widestColor[i]];
                int endPosToAnalyze = colorEndPos[widestColor[i]];
                int startPosHops = 0;
                int endPosHops = 0;
                for (int j = startPosToAnalyze + 1; j <= endPosToAnalyze - 1; j++)
                {
                    if (output[j] == stripes[j])
                        startPosHops++;
                    else
                        break;
                }
                for (int j = endPosToAnalyze - 1; j >= startPosToAnalyze + 1 + startPosHops; j--)
                {
                    if (output[j] == stripes[j])
                        endPosHops++;
                    else
                        break;
                }

                numSweeps += PaintOptSubs(stripes, output, 0, startPosToAnalyze - 1, numSweeps, localSolutionLog, debug);
                numSweeps += PaintOptSubs(stripes, output, startPosToAnalyze + 1 + startPosHops, endPosToAnalyze - 1 - endPosHops, numSweeps, localSolutionLog, debug);
                numSweeps += PaintOptSubs(stripes, output, endPosToAnalyze + 1, stripes.Length - 1, numSweeps, localSolutionLog, debug);

                if (numSweeps < minNumSweeps)
                {
                    minNumSweeps = numSweeps;
                    solutionLog = localSolutionLog;
                }
            }

            if (debug)
                Console.WriteLine(solutionLog);

            return minNumSweeps == 100000 ? 0 : minNumSweeps;
        }

        static int PaintOptSubs(string stripes, char[] parentOutput, int startPosToAnalyze, int endPosToAnalyze, int numSweeps, StringBuilder parentSolutionLog, bool debug)
        {
            if (endPosToAnalyze < startPosToAnalyze)
                return 0;
            else if (endPosToAnalyze - startPosToAnalyze <= 1)
                return ChangeSmallestBlock(stripes, parentOutput, startPosToAnalyze, endPosToAnalyze, numSweeps, parentSolutionLog, debug);

            stackSize++;
            if (stackSize > 50)
                throw new Exception("Stack size too big");

            char[] parentOutputCopy = new char[parentOutput.Length];
            parentOutput.CopyTo(parentOutputCopy, 0);
            int[] colorStartPos = new int[26] { 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99 };
            int[] colorEndPos = new int[26] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
            int[] widestColor = new int[26] { -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1 };
            StringBuilder finalLocalSolutionLog = new StringBuilder();
            //Greedy choice:
            ChooseNextStripesGreedy(stripes, startPosToAnalyze, endPosToAnalyze, colorStartPos, colorEndPos, widestColor);

            int minNumSweeps = 100000;
            int threadId = 1;
            for (int i = 0; i < 10 && widestColor[i] != -1; i++, threadId++)
            {
                int localNumSweeps = 0;
                int newStartPosToAnalyze = colorStartPos[widestColor[i]];
                int newEndPosToAnalyze = colorEndPos[widestColor[i]];
                int startPosHops = 0;
                int endPosHops = 0;
                char[] localOutput = new char[parentOutput.Length];
                parentOutput.CopyTo(localOutput, 0);
                StringBuilder localSolutionLog = new StringBuilder();

                bool mustBePainted = true;
                if (localOutput[newStartPosToAnalyze] == widestColor[i] + 65) //it means the whole section is already painted with that color
                    mustBePainted = false;

                if (mustBePainted)
                {
                    for (int j = newStartPosToAnalyze; j <= newEndPosToAnalyze; j++)
                        localOutput[j] = Convert.ToChar(widestColor[i] + 65);
                    localNumSweeps++;

                    if (debug)
                        BuildSolutionLog(localOutput, numSweeps + localNumSweeps, localSolutionLog);
                }

                for (int j = newStartPosToAnalyze + 1; j <= newEndPosToAnalyze - 1; j++)
                {
                    if (localOutput[j] == stripes[j])
                        startPosHops++;
                    else
                        break;
                }
                for (int j = newEndPosToAnalyze - 1; j >= newStartPosToAnalyze + 1 + startPosHops; j--)
                {
                    if (localOutput[j] == stripes[j])
                        endPosHops++;
                    else
                        break;
                }

                localNumSweeps += PaintOptSubs(stripes, localOutput, startPosToAnalyze, newStartPosToAnalyze - 1, numSweeps + localNumSweeps, localSolutionLog, debug);
                localNumSweeps += PaintOptSubs(stripes, localOutput, newStartPosToAnalyze + 1 + startPosHops, newEndPosToAnalyze - 1 - endPosHops, numSweeps + localNumSweeps, localSolutionLog, debug);
                localNumSweeps += PaintOptSubs(stripes, localOutput, newEndPosToAnalyze + 1, endPosToAnalyze, numSweeps + localNumSweeps, localSolutionLog, debug);
                if (localNumSweeps < minNumSweeps)
                {
                    localOutput.CopyTo(parentOutputCopy, 0);
                    finalLocalSolutionLog = localSolutionLog;
                    minNumSweeps = localNumSweeps;
                }
            }

            parentOutputCopy.CopyTo(parentOutput, 0);
            minNumSweeps = minNumSweeps == 100000 ? 0 : minNumSweeps;
            parentSolutionLog.Append(finalLocalSolutionLog);
            stackSize--;
            return minNumSweeps;
        }

        static int ChangeSmallestBlock(string stripes, char[] output, int startPosToAnalize, int endPosToAnazlize, int numSweeps, StringBuilder solutionTree, bool debug)
        {
            if (startPosToAnalize == endPosToAnazlize)
            {
                if (output[startPosToAnalize] != stripes[startPosToAnalize])
                {
                    output[startPosToAnalize] = stripes[startPosToAnalize];
                    if (debug)
                        BuildSolutionLog(output, numSweeps + 1, solutionTree);

                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else if (startPosToAnalize + 1 == endPosToAnazlize)
            {
                if (output[startPosToAnalize] == stripes[startPosToAnalize] && output[endPosToAnazlize] == stripes[endPosToAnazlize])
                {
                    return 0;
                }
                else if (output[startPosToAnalize] == stripes[startPosToAnalize] && output[endPosToAnazlize] != stripes[endPosToAnazlize])
                {
                    output[endPosToAnazlize] = stripes[endPosToAnazlize];
                    if (debug)
                        BuildSolutionLog(output, numSweeps + 1, solutionTree);
                    return 1;
                }
                else if (output[startPosToAnalize] != stripes[startPosToAnalize] && output[endPosToAnazlize] == stripes[endPosToAnazlize])
                {
                    output[startPosToAnalize] = stripes[startPosToAnalize];
                    if (debug)
                        BuildSolutionLog(output, numSweeps + 1, solutionTree);
                    return 1;
                }
                else if (output[startPosToAnalize] != stripes[startPosToAnalize] && output[endPosToAnazlize] != stripes[endPosToAnazlize] &&
                    stripes[startPosToAnalize] != stripes[endPosToAnazlize])
                {
                    output[startPosToAnalize] = stripes[startPosToAnalize];
                    if (debug)
                        BuildSolutionLog(output, numSweeps + 1, solutionTree);
                    output[endPosToAnazlize] = stripes[endPosToAnazlize];
                    if (debug)
                        BuildSolutionLog(output, numSweeps + 2, solutionTree);
                    return 2;
                }
                else if (output[startPosToAnalize] != stripes[startPosToAnalize] && output[endPosToAnazlize] != stripes[endPosToAnazlize] &&
                    stripes[startPosToAnalize] == stripes[endPosToAnazlize])
                {
                    output[startPosToAnalize] = stripes[startPosToAnalize];
                    output[endPosToAnazlize] = stripes[endPosToAnazlize];
                    if (debug)
                        BuildSolutionLog(output, numSweeps + 1, solutionTree);
                    return 1;
                }
                else
                {
                    Console.WriteLine("ERROR XX in Function ChangeSmallestBlock");
                    return 0;
                }
            }
            else
            {
                Console.WriteLine("ERROR YY in Function ChangeSmallestBlock");
                return 0;
            }
        }

        static void ChooseNextStripesGreedy(string stripes, int startPosToAnalize, int endPosToAnazlize, int[] colorStartPos, 
            int[] colorEndPos, int[] arrWidestColor)
        {
            int widestColor = -1;
            int widestColorWidth = -1;
            int widestColorStartPos = 99;
            int widestColorEndPos = -1;

            for (int i = startPosToAnalize; i <= endPosToAnazlize; i++)
            {
                int color = Convert.ToInt32(stripes[i]) - 65;
                if (i < colorStartPos[color])
                    colorStartPos[color] = i;
                if (i > colorEndPos[color])
                    colorEndPos[color] = i;

                if (colorEndPos[color] - colorStartPos[color] + 1 > widestColorWidth)
                {
                    widestColor = color;
                    widestColorStartPos = colorStartPos[color];
                    widestColorEndPos = colorEndPos[color];
                    widestColorWidth = colorEndPos[color] - colorStartPos[color] + 1;
                }
            }

            arrWidestColor[0] = widestColor;

            if (widestColorWidth <= 2)
            {
                return;
            }
            else
            {
                int arrIndex = 1;
                for (int i = 0; i < 26; i++)
                {
                    if (colorEndPos[i] - colorStartPos[i] <= 1)
                        continue;

                    if (i == widestColor)
                        continue;

                    //if ((colorStartPos[i] > widestColorStartPos && colorEndPos[i] < widestColorEndPos) ||
                    //    (colorStartPos[i] > widestColorEndPos) ||
                    //    (colorEndPos[i] < widestColorStartPos))
                    //    continue;
                    if (
                        (colorStartPos[i] > widestColorEndPos) ||
                        (colorEndPos[i] < widestColorStartPos))
                        continue;

                    arrWidestColor[arrIndex++] = i;
                }
            }
        }

        static void ProcessFile(string inputfilename)
        {
            int counter = 0;
            string line;
            string outputfilename = inputfilename + ".output.txt";

            // Read the file and display it line by line.
            try
            {
                System.IO.StreamReader inputFile = new System.IO.StreamReader(inputfilename);
                System.IO.StreamWriter outputFile = new System.IO.StreamWriter(outputfilename);

                while ((line = inputFile.ReadLine()) != null)
                {
                    counter++;
                    int numChanges = Paint(line);
                    outputFile.WriteLine(numChanges);
                }

                inputFile.Close();
                outputFile.Close();

                Console.WriteLine("Output file saved as \"" + outputfilename + "\"");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void BuildSolutionLog(char[] output, int numSweeps, StringBuilder solutionTree)
        {
            if (numSweeps < 10)
                solutionTree.Append("0" + numSweeps + ": " + new string(output) + Environment.NewLine);
            else
                solutionTree.Append(numSweeps + ": " + new string(output) + Environment.NewLine);
        }

    }
}
