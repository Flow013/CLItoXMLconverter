# CLItoXMLconverter
Разработать приложение для конвертирования приложений JunOS из формата JunOS CLI в формат JunOS XML, с возможностью промежуточного просмотра.
Образцы файлов приведены в приложениях. Обратите внимание на отличие приложений (application) от групп приложений (application-set).


**Требования к реализации:**
* Среда разработки: Visual Studio Community, язык: C#, тип приложения: WPF.
* Для реализации интерфейса необходимо использовать паттерн MVVM.
*	Расположение элементов управления должно соответствовать рисунку 1.
*	Предусмотреть возможность сортировки по столбцам.
*	Предусмотреть возможность автоматической генерации столбцов таблицы на основе модели (используя Reflections). Что бы при изменении модели – отображение изменялось соответственно.
*	При парсинге формата CLI необходимо использовать регулярные выражения.
*	Если во время парсинга встречаются неизвестные строки, они должны игнорироваться.
*	Если пользователь выбрал неверный файл (например: картинку), должно выводиться сообщение об ошибке.
*	При экспорте в XML можно использовать любую библиотеку. Генерировать его вручную нет необходимости.

![Рисунок 1: Эскиз интерфейса.](https://image.ibb.co/heEBt9/converter.png)
<p align="center">
Рисунок 1: Эскиз интерфейса.
</p>

**Приложение 1: Образец формата JunOS CLI**
```cli
set applications application custom-sql protocol tcp
set applications application custom-sql destination-port 5000-6000
set applications application custom-xyz protocol udp
set applications application custom-xyz source-port 8888
set applications application custom-xyz destination-port 5500
set applications application custom-xyz description myCoolApp
set applications application-set MgrAppSet application junos-ssh
set applications application-set MgrAppSet application junos-telnet
set applications application-set MgrAppSet application junos-https
set applications application-set MgrAppSet application junos-http
set applications application-set MgrAppSet application custom-sql
set applications application-set GoodApps application custom-xyz
set applications application-set GoodApps application-set MgrAppSet
```


**Приложение 2: Образец формата JunOS XML**
```xml
<applications>
                <application>
                    <name>custom-sql</name>
                    <protocol>tcp</protocol>
                    <destination-port>5000-6000</destination-port>
                </application>
                <application>
                    <name>custom-xyz</name>
                    <protocol>udp</protocol>
                    <source-port>8888</source-port>
                    <destination-port>5500</destination-port>
                    <description>myCoolApp</description>
                </application>
                <application-set>
                    <name>MgrAppSet</name>
                    <application>
                        <name>junos-ssh</name>
                    </application>
                    <application>
                        <name>junos-telnet</name>
                    </application>
                    <application>
                        <name>junos-https</name>
                    </application>
                    <application>
                        <name>junos-http</name>
                    </application>
                    <application>
                        <name>custom-sql</name>
                    </application>
                </application-set>
                <application-set>
                    <name>GoodApps</name>
                    <application>
                        <name>custom-xyz</name>
                    </application>
                    <application-set>
                        <name>MgrAppSet</name>
                    </application-set>
                </application-set>
            </applications>
```
