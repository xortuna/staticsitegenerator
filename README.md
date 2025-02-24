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
/assets/css/
/assets/js/
/assets/image/
/blog/index.html
/blog/posts/mypost1/index.html
/blog/posts/mypost2/index.html
```

## Variables
The templating engine has a rather crude and simplistic function resolving system. The most common syntax you'll use is the include("myhtml.html") function to embed other html files into your html file.

## Example HTML using templates

``` HTML
<!DOCTYPE html>
<html lang="en">

<head>
  <!-- Global header includes (JS files, fonts etc) -->
  {{include('_header.html')}}
  <meta name="description" content="List of blog posts published by Piste">
  <title>Piste: Blog</title>
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
