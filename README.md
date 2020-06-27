# LineparineOneToManyJsonDictionary

![](https://raw.githubusercontent.com/skytomo221/LineparineOneToManyJsonDictionary/master/images/thumbnail.png)

LineparineOneToManyJsonDictionaryは、lineparine.dicをOTM-JSON形式に変換するプログラムです。

## 導入方法

1. `git clone https://github.com/skytomo221/LineparineOneToManyJsonDictionary`
2. Visual Studio > ビルド(B) > ソリューションのビルド(B)
3. `.\LineparineOneToManyJsonDictionary\bin\Debug` または `.\LineparineOneToManyJsonDictionary\bin\Release` に `lineparine.csv` を置く
4. `lineparine.csv` を置いたディレクトリで、 `.\LineparineOneToManyJsonDictionary.exe` を実行

### lineparine.csv の作成方法

1. [リパライン語・日本語辞書](https://sites.google.com/site/3tvalineparine/pei-bu#TOC--9)で lineparine.dic をダウンロード
2. [PDIC](http://pdic.la.coocan.jp/) に lineparine.dic を登録
3. [PDIC](http://pdic.la.coocan.jp/) > File > 辞書設定<詳細>(E) > 登録した lineparine.dic を右クリック > 辞書の変換
4. 変換ファイル形式を「CSV形式」に設定してOK

## 機能

- 訳語検索
- 古語、口語、略語にタグが付く
- 国際理語にタグが付く
- 語法と文化が別の項目として表示される
- 小見出しの追加
- 格変化の追加

## ライセンス

このプログラム自体はMITライセンスですが、
使用される辞書データの全ての権利は辞書データの作成者に帰属することに注意してください。
