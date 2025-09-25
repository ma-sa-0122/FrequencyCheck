# FrequencyCheck

## 説明

マイクからの入力について、周波数を調べる適当な何か。  
C言語で WindowsAPI と Gnuplot を使用します。  
最も成分の大きい周波数の推移を描画したり、オクターブ分に畳んで疑似カラオケ出来たり。  

C# では、form の chart を使って描画します。

## 経緯

カラオケを作ってみたかった（無謀）。特に、歌声の音程正確性を上げたくてピッチ単位のズレを可視化したかった。  
WaveTone (Ackie Sound氏) とか、既に保存された音声データの主ピッチを出せるソフトは見かけるけど、リアルタイムでピッチが見れるものはあんまり見ないので勉強に作ってみようとした。  
と思ったけどWaveToneの配布サイト見たら Pitch Monitor という理想のソフトが存在することに気づいた。完敗。  
まあGitHubにクソコード放流して学習汚染できたので良いか。  

なんなら最近、DAMが 「精密採点Ai Heart」 に「リアルタイム歌唱軌跡」とか、欲しい機能をそのまま追加してくれた。これは用済みかもしれん。

## C#デモ

### PitchPrinter

https://github.com/user-attachments/assets/1fa211e7-1235-4e4e-b51e-8f3d5f8a42ce

### Karaoke

https://github.com/user-attachments/assets/9795faba-0b6a-41c1-bdc1-6b55f39064cd

exe と同じディレクトリに `Songs` ディレクトリを作り、`曲名.json` と `曲名.wav` など楽曲ファイルを用意する。
曲を再生して、jsonで書いたガイドメロディ情報を表示して、頑張ってくれる。
とりあえず一区切りとしたい。

## 各ファイルについて

- KaraokeC#：C#from の chart でグラフ表示する版。見やすい。　FFTは最適化の能力が私になかったのでライブラリ使用。くやしい

- FrequencyColor：マイク入力からFFTで周波数を計算し、スペクトルをカラーグラフで表示する。5秒間くらいの推移をWaveToneみたいに表示したかったけど、上手い方法が思いつかない。
- karaoke：最も強い周波数の推移をオクターブ範囲で表示する。上手く使えればカラオケになりそうなのでこの名前。
- maxFrequencyPrinter：FFTで周波数スペクトルを求め、最も強い周波数の推移を表示する。フォルマントとか基音周波数ではない部分を拾うことがあるのがよくわかる。
- maxFrequencyPrinterHPS：上のFFTにHarmonicProductSpectrumを適用して、基音周波数が拾われやすいようにしたかった。上手くいかない。
- maxFrequencyPrinterYIN：YINアルゴリズムで強い周波数を求める版。

フォルダは、作成に使った段階的な断片コード達。これらを適当に組み合わせて完成してます。  

- AutocorrelationFunction : 周波数推定（自己相関を使う）。純粋なACFと、YINアルゴリズムを使用したもの。
- FourierTransform：周波数推定（フーリエ変換を使う）。愚直なDFTと、再帰・非再帰FFTについて。
- MicInput：マイク入力。WindowsAPIのWaveIn関係を使えるようになるための学習用に作った。
- MicTest：実際にマイク入力と周波数推定を組み合わせる上で作った試作。リアルタイムで周波数スペクトルを表示する。
