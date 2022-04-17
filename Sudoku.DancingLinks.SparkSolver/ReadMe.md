# Projet C#

## Résolution de sudokus avec Dancinlinks

### 1- Introduction

Nous nous sommes inspiré du repo https://github.com/jsboigeECE/ECE-2022-Ing4-Finance-IA1-Gr02/tree/main/Sudoku.DancingLinksSolvers pour intégrer le code de résolution de sudoku avec la méthode des DancinLinks

### 2- Modifications apportées au code original

#### 2.1 Adaptation pour prise en compte d'un fichier csv

Après avoir défini une variable filePath (qui se trouve au dessus du main()) poitant vers le fichier csv contenant les sudokus. Les sudokus dans le fichier csv étaient sous la forme horizontale, il a été nécéssaire de faire appel à la fonction GridSudoku() pour créer MatrixList nécéssaire pour le constructeur.
```c#
var filePath = Path.Combine(Environment.CurrentDirectory, "sudoku.csv");
```
#### 2.2 Initiationsation de la SparkSession depuis le main(), avec paramètres sur le nombre de cores et de workers.
création de la SparkSession, du DataFrame, de l'UDF, résolution des sudokus
```c#
static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            var nbWorkers = 4;
            var nbCoresPerWorker = 1;
            var nbLignesMax = 100000;

            var chronometre = Stopwatch.StartNew();

            var filePath = Path.Combine(Environment.CurrentDirectory, "sudoku.csv");
            Utils.PrepareSudokus();


            SparkSession spark =
                SparkSession
                    .Builder()
                    .AppName("Norvig Solver Spark")
                    .Config("spark.executor.cores", nbCoresPerWorker)
                    .Config("spark.executor.instances", nbWorkers)
                    .GetOrCreate();

            DataFrame dataFrame = spark
                .Read()
                .Option("header", true)
                //.Option("inferSchema", true)
                .Schema("quizzes string, solutions string")
                .Csv(filePath);

            DataFrame milleSudoku = dataFrame.Limit(nbLignesMax);


            spark.Udf().Register<string, string>(
                "SukoduUDF",
                (sudoku) => SolveSudoku(sudoku));

            milleSudoku.CreateOrReplaceTempView("Resolved");
            DataFrame sqlDf = spark.Sql("SELECT quizzes, SukoduUDF(quizzes) as Resolution from Resolved");
            sqlDf.Collect();
            sqlDf.Show();

            var tempsEcoule = chronometre.Elapsed;
            Console.WriteLine($"temps d'execution: {tempsEcoule.ToString()}");

        }
```


### 3- Résultats obtenus
On a choisi 1000000 sudokus dans sudoku.csv et j'ai utilisé 5800x (8 cores) comme le CPU expérimental

#### 3.1 4 workers et 1 Core per worker 
var nbWorkers = 4;
var nbCoresPerWorker = 1;
```c#
+--------------------+--------------------+
|             quizzes|          Resolution|
+--------------------+--------------------+
|00430020900500900...|86437125932584976...|
|04010005010700396...|34617925818752396...|
|60012038400845907...|69512738413845967...|
|49720000010040000...|49725831618643972...|
|00591030800940306...|46591237818947356...|
|10000500738090000...|19468523738297451...|
|00906543000700080...|28976543131792485...|
|00000065770240010...|89423165776249518...|
|50307019000000675...|56347219821938675...|
|06072090808400300...|16372594858469327...|
|00408300205100430...|97418365265127438...|
|00006028070900100...|43156728972948165...|
|00430000089020067...|25436789189321567...|
|00807010012009005...|95827416312369875...|
|06537000200000137...|86537941292458137...|
|00571032900036280...|86571432991736284...|
|20000530000007385...|26849531719467385...|
|04080050008076009...|94781256358376419...|
|05008301700010040...|65248391797816243...|
|70008400530070102...|71298436534675182...|
+--------------------+--------------------+
only showing top 20 rows

temps d'execution: 00:00:04.6940120
```
#### 3.2 4 workers et 2 Core per worker 
var nbWorkers = 4;
var nbCoresPerWorker = 2;
```c#
+--------------------+--------------------+
|             quizzes|          Resolution|
+--------------------+--------------------+
|00430020900500900...|86437125932584976...|
|04010005010700396...|34617925818752396...|
|60012038400845907...|69512738413845967...|
|49720000010040000...|49725831618643972...|
|00591030800940306...|46591237818947356...|
|10000500738090000...|19468523738297451...|
|00906543000700080...|28976543131792485...|
|00000065770240010...|89423165776249518...|
|50307019000000675...|56347219821938675...|
|06072090808400300...|16372594858469327...|
|00408300205100430...|97418365265127438...|
|00006028070900100...|43156728972948165...|
|00430000089020067...|25436789189321567...|
|00807010012009005...|95827416312369875...|
|06537000200000137...|86537941292458137...|
|00571032900036280...|86571432991736284...|
|20000530000007385...|26849531719467385...|
|04080050008076009...|94781256358376419...|
|05008301700010040...|65248391797816243...|
|70008400530070102...|71298436534675182...|
+--------------------+--------------------+
only showing top 20 rows

temps d'execution: 00:00:00.9456051
```

#### Conclusion

DacinLinks est déjà très rapide et bien optimisé pour résoudre les sudokus avec une différence entre (1 core et 4 workers) et (2 cores et 4 workers) notable sur 1000000 sudokus.

Pour 1000000 sudokus, la différence entre (1 core et 4 workers) et (2 cores et 4 workers) passe à 3 secondes pour la résolution.

On en conclu que plus le nombre de sudoku à résoudre sera grand, plus la différence entre (1 core et 4 workers) et (2 cores et 4 workers) sera notable. 

### 4 - Code d'exécution dans le terminal (Windows) pour lancer le projet avec Spark-Submit
cd .\Sudoku.DancingLinks.SparkSolver\bin\Debug\netcoreapp3.1\

spark-submit --class org.apache.spark.deploy.dotnet.DotnetRunner --master local microsoft-spark-3-0_2.12-2.1.0.jar debug

Après avoir entré le code, vous pouvez voir les résultats.
