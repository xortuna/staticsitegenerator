# Static Site Generator for C# #

Dirt simple static site generator

Used to generate the https://pisteapp.com website

## Command Line

```
StaticSiteGenerator.exe -w -x "https://mysite.com"
```

## Arguments
| Argument      | Description      |
| ------------- | ------------- |
| -w | Watch the input folder for changes |
| -mi | Dont auto detect multi resolution images |
| -t | Add additional asset file types (DEF: ".css", ".png", ".svg", ".js", ".webp", ".mp4", ".webm") |
| -x "https://mysite.com" | Set the Base URL and generate a sitemap.xml |

## Templates

Any files or folders starting with an underscore are not processed.
Theres a global template directory called _partial, and a special __template.html file for markdown files. Other than that you can define whatever structure you like.

Example Structure for a blog
```
/_partial/_header.html
/_partial/_footer.html
/_partial/_navbar.html
/assets/css/
/assets/js/
/assets/image/
/blog/index.html
/blog/posts/__template.html
/blog/posts/mypost1.md
/blog/posts/mypost2.md
```

When Running the tool a new folder called _www will be created, any assets or HTML files will be copied into the relevent directories, and the Markdown files will be parsed and exploded into subdirectories

```
_www/assets/css/
_www/assets/js/
_www/assets/image/
_www/blog/index.html
_www/blog/posts/mypost1/index.html
_www/blog/posts/mypost2/index.html
```

## Variables
The templating engine has a rather crude and simplistic function resolving system. The most common syntax you'll use is the include("myhtml.html") function to embed other html files into your html file.

## Arguments

### Numeric
| Function | Arguments | Returns  | Description   |
| ------------- | ------------- | ------------- | ------------- |
| Add | number1,number2 | number | Adds two numbers together and returns the result |
| Subtract | number1,number2 | number | Substracts two numbers together and returns the result |
| Divide | number1,number2 | number | Divides two numbers together and returns the result |
| Multiply | number1,number2 | number | Multiplys two numbers together and returns the result |

### Variables
| Function | Arguments | Returns  | Description   |
| ------------- | ------------- | ------------- | ------------- |
| Assign | VariableName,value | - | Assigns VariableName to the specified value |
| Var | VariableName | value | Retrives the value stored under the variable name  |

Variables can also assigned using metadata flags in HTML and Markdown files, this is particually usefull when you want to get a summary information of a file without nesserially parsing the whole file.
```
{#VariableName:value#}
```

| Function | Arguments | Returns  | Description   |
| ------------- | ------------- | ------------- | ------------- |
| load_metadata | input filename,body | result of body | Loads the metadata variables from a file into the stack then executes the body statement|

### Conditionals
| Function | Arguments | Returns  | Description   |
| ------------- | ------------- | ------------- | ------------- |
| equals | LHS,RHS | bool | Returns true if the result of LHS equals the result of RHS  |
| doesnotequal | LHS,RHS | bool | Returns true if the result of LHS does not equal the result of RHS  |


### String opertaions
| Function | Arguments | Returns  | Description   |
| ------------- | ------------- | ------------- | ------------- |
| Concat | Str1,Str2,... | String | Concatinates all the paramaters into one string |


### Arrays
| Function | Arguments | Returns  | Description   |
| ------------- | ------------- | ------------- | ------------- |
| foreach | array,body,[foreachvariablename(def=foreach)] | array result of body | executes the body for each item in an array, foreach.key and foreach.index are assigend onto the stack|


## Example HTML using templates
In the spirit of Razor, you can drop in and out of the templating syntax using the escaping sequence {{ }} 

/blog/index.html - Shows a list of recent blog posts
``` HTML
{#page.title:Blog Posts#}
<!DOCTYPE html>
<html lang="en">

<head>
  <!-- Global header includes (JS files, fonts etc) -->
  {{include('_header.html')}}
  <meta name="description" content="List of blog posts published by Piste">
  <title>Piste: {{var(page.title)}}</title>
</head>

<body class="d-flex flex-column min-vh-100">
  <!-- Static Top Menu -->
  {{include('_navbar.html')}}

  <!-- Content -->
  <div class="content container my-4">
    <!-- Breadcrumb -->
    <nav aria-label="breadcrumb">
      <ol class="breadcrumb">
        <li class="breadcrumb-item"><a href="/">Home</a></li>
        <li class="breadcrumb-item active" aria-current="page">Blog</li>
      </ol>
    </nav>

    <h1>New Ski Blog Posts</h1>
    <p>Sometimes we write stuff about skiing and snowboarding, others we write about whatever takes our fancy! These are
      our latest and greatest articles for you to check out.</p>
    <div class="row">
      {{foreach(list_files('/blog/posts','*.md'),load_metadata(var('foreach.key'), include('_blogpost_stub.html')))}}
    </div>
  </div>

  <!-- Footer -->
  {{include('_footer.html')}}
</body>
</html>
```

## Example Markdown ##

Markdown files can be used to ease of content creation without the hassle of authoring HTML files. The markdown files get combined with a __template.html file to create a html sub directory.

e.g mypost.md gets transformed into /mypost/index.html

/blog/posts/mypost1.md An example blog post with meta data prefixed (the metadata is used in the template to set things like the page title etc)
```MARKDOWN
{#post.title:Top 5 Ski Runs in Val d'Isère#}
{#post.date:16 September 2024#}
{#post.resort:Tignes Val d'Isere#}
{#post.caption:Did your favorite run make the list?#}
{#post.image:/assets/image/blog/topskirunvaldisere/header.png#}
# Top 5 Ski Runs in Val d'Isère

Heading off to Val d'Isère or Tignes this year? Don't miss these top runs. From beginner green to icy half pipes we think you'll find a favourite in our mix.

## 5. Glacier - Val d'Isere

![Glacier](/assets/image/blog/topskirunvaldisere/5.webp)

![float-end inline-image][Map of Glacier Piste](/assets/image/blog/topskirunvaldisere/5_map.png)

Val d'Isere has its own Glacier to rival Tignes's Grand Motte. A high-altitude gem that offers spectacular skiing on pristine snow, thanks to its location atop the Pissaillas Glacier. As a blue run, it provides a wide, gentle descent, making it ideal for intermediate skiers while offering stunning panoramic views of the surrounding peaks. The reliable snow conditions and breathtaking scenery make it a must-visit, particularly in the early season when lower-altitude runs may lack coverage. It’s a smooth, enjoyable ride with excellent snow, perfect for skiers looking to experience glacier skiing.
```

/blog/posts/__template.html an example template for all blog posts to compile into

```HTML
<!DOCTYPE html>
<html lang="en">
<head>
  {{include('_header.html')}}
  <meta name="description" content="{{var('post.caption')}}">
  <title>{{var('post.title')}}</title>
  <link href="/assets/css/piste_blog.css" rel="stylesheet">
</head>

<body class="d-flex flex-column min-vh-100">
  <!-- Static Top Menu -->
  {{include('_navbar.html')}}

  <!-- Content -->
  <div class="content container my-4">
    <!-- Breadcrumb -->
    <div class="row">
      <div class="col-md-8">
        <nav aria-label="breadcrumb">
          <ol class="breadcrumb">
            <li class="breadcrumb-item"><a href="/">Home</a></li>
            <li class="breadcrumb-item" aria-current="page"><a href="/blog/">Blog</a></li>
            <li class="breadcrumb-item active" aria-current="page">{{var('post.title')}}</li>
          </ol>
        </nav>
      </div>
      <div class="col-md-4">
        <small class="float-end">Posted {{var('post.date')}}</small>
      </div>
    </div>
    <div class="row">
      <div class="col-md-8">
        <!-- Insert the markdown body here -->
        {{var('content')}}
      </div>
      <div class="col-md-4 d-none d-md-block">
        <h2>Related Posts</h2>
        {{
        foreach(list_files('/blog/posts','*.md').where(notequal(var('where.key'),var('input.fullname'))).take(5),
        load_metadata(var('foreach.key'),include('_blogpost_stub_horizontal.html')))}}
      </div>
    </div>
  </div>

  <!-- Footer -->
  {{include('_footer.html')}}
</body>
</html>
```
