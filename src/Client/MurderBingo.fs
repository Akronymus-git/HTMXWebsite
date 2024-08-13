module Client.MurderBingo

let murderbingo = """<!-- Thanks to:
- http://davidbau.com/archives/2010/01/30/random_seeds_coded_hints_and_quintillions.html
- https://stackoverflow.com/a/2450976/174466
-->
<script>
// seedrandom
!function(a,b,c,d,e,f,g,h,i){function j(a){var b,c=a.length,e=this,f=0,g=e.i=e.j=0,h=e.S=[];for(c||(a=[c++]);d>f;)h[f]=f++;for(f=0;d>f;f++)h[f]=h[g=s&g+a[f%c]+(b=h[f])],h[g]=b;(e.g=function(a){for(var b,c=0,f=e.i,g=e.j,h=e.S;a--;)b=h[f=s&f+1],c=c*d+h[s&(h[f]=h[g=s&g+b])+(h[g]=b)];return e.i=f,e.j=g,c})(d)}function k(a,b){var c,d=[],e=typeof a;if(b&&"object"==e)for(c in a)try{d.push(k(a[c],b-1))}catch(f){}return d.length?d:"string"==e?a:a+"\0"}function l(a,b){for(var c,d=a+"",e=0;e<d.length;)b[s&e]=s&(c^=19*b[s&e])+d.charCodeAt(e++);return n(b)}function m(c){try{return o?n(o.randomBytes(d)):(a.crypto.getRandomValues(c=new Uint8Array(d)),n(c))}catch(e){return[+new Date,a,(c=a.navigator)&&c.plugins,a.screen,n(b)]}}function n(a){return String.fromCharCode.apply(0,a)}var o,p=c.pow(d,e),q=c.pow(2,f),r=2*q,s=d-1,t=c["seed"+i]=function(a,f,g){var h=[];f=1==f?{entropy:!0}:f||{};var o=l(k(f.entropy?[a,n(b)]:null==a?m():a,3),h),s=new j(h);return l(n(s.S),b),(f.pass||g||function(a,b,d){return d?(c[i]=a,b):a})(function(){for(var a=s.g(e),b=p,c=0;q>a;)a=(a+c)*d,b*=d,c=s.g(1);for(;a>=r;)a/=2,b/=2,c>>>=1;return(a+c)/b},o,"global"in f?f.global:this==c)};if(l(c[i](),b),g&&g.exports){g.exports=t;try{o=require("crypto")}catch(u){}}else h&&h.amd&&h(function(){return t})}(this,[],Math,256,6,52,"object"==typeof module&&module,"function"==typeof define&&define,"random");
</script>
<html>
  <head>
    <style>
      body {
        margin: 8px;
        background-color: #333;
        color: #ccc;
      }
      td {
        border: 1px solid #CCC;
        vertical-align: middle;
        text-align: center;
        aspect-ratio: 1;
        cursor: pointer;
        user-select: none;
        transform: rotate(1deg);
      }
      th {
        font-size: 2rem;
        padding: 1rem;
        height: 
      }
      thead {
       background-color: #ccc;
        color: #333;
        height: 100px
      }
      tbody {
      }
      table{
        border-collapse: collapse;
        height: 100%;
        
        }
      .won {
        background: #919191;
        color: #000;
      }
      #root {
        height: 100vh;
      }
    </style>
  </head>
  <body>
    <noscript>
      <p>The bingo requires Javascript to be enabled.</p>
    </noscript>
    <div id="root">
      <form id="form">
        <p><label>Enter your exact Twitch name: <input type="text" id="seed" /></label></p>
        <p><input type="submit" value="Generate!" /></p>
      </form>
    </div>
  </body>
</html>
<script>
const list = ["Bro killed by Bandit/Nomad","Bro swallowed whole by Nachzehrer","Bro becomes undead","Bro killed by Unhold","Bro killed by Goblin","Bro killed by Orc","Bro killed by Barbarian","Bro killed by Necrosavant","Bro killed by Friendly Fire","Bro killed by Direwolf","Bro killed by Hyena","Bro bleeds out","Bro killed by Webknecht","Bro killed by Alp","Bro killed by Serpent","Bro deserts company","Bro beheaded","Bro gets brain damage","Bro loses eye","Bro loses ear","Bro loses nose","Bro loses finger","4+ fall in one battle","8+ fall in one battle","Total Party Wipe","We raid a caravan","We take an enemy camp","We raid peasants","Have 10K Crowns","Get a Famed weapon","Get a Famed bodyarmour","Bro reaches lv.8","One-shot Multikill","Our dog dies","Our dog kills","Have 2 retinues","Bro reaches lv.11","Kill an Unhold","Kill a Necrosavant","Kill a Hexen","Kill a Lindwurm","Kill an Alp","Kill a Schrat","Have a 250+ head armour piece","Have a 250+ body armour piece","Kill an Orc Warlord","Kill a Necromancer","Defeat another Mercenary Company","Get Famed Helmet","Get Famed Shield","Complete a 3-skull contract","Kill a Barbarian King","Win against 20 enemies","Win against 30 enemies","Win an arena fight"]
const width = 7
const height = 7
const title = "Battle Brothers Bingo"
const freespace = {"text":"A Bro Dies (FREE SPACE)","description":"","placement":"CENTER"}
const saveload = true

document.title = title

function shuffle(array) {
  var currentIndex = array.length, randomIndex;

  // While there remain elements to shuffle...
  while (currentIndex != 0) {

    // Pick a remaining element...
    randomIndex = Math.floor(Math.random() * currentIndex);
    currentIndex--;

    // And swap it with the current element.
    [array[currentIndex], array[randomIndex]] = [
      array[randomIndex], array[currentIndex]];
  }

  return array;
}

let seed
let localstoragekey
const form = document.getElementById("form")
form.addEventListener("submit", e => {
  e.preventDefault()

  const seedel = document.getElementById("seed")
  seed = seedel.value
  if (seed == "") { seed = seedel.placeholder }
  loadseed()

  return false
})

const set_qs = (key, value) => {
  const params = new URLSearchParams(window.location.search)
  if (value === null) {
    params.delete(key)
  } else {
    params.set(key, value)
  }
  window.history.replaceState(null, null, "?" + params)
}

const loadseed = (push_history = true) => {  
  Math.seedrandom(seed)
  const shuffled = [ ...list ]
  shuffle(shuffled)
  renderBingo(shuffled)
  localstoragekey = `${title}`
  load()
  if (push_history) {
    set_qs("seed", seed)
  }
}

const renderBingo = list => {
  if (freespace !== undefined) {
    let idx;
    switch (freespace.placement) {
      case "CENTER":
        center_x = Math.floor(width / 2)
        center_y = Math.floor(height / 2)
        idx = center_y * width + center_x
        break
      case "RANDOM":
        idx = Math.floor(Math.random() * width * height)
        break
    }
    list[idx] = freespace
  }
  const table = document.createElement("table")
  {
    const thead = document.createElement("thead")
    const tr = document.createElement("tr")
    const th = document.createElement("th")
    th.colSpan = width
    th.innerText = `${title} - seed: ${seed}`
    tr.appendChild(th)
    thead.appendChild(tr)
    table.appendChild(thead)
  }
  const tbody = document.createElement("tbody")
  for (y = 0; y < height; y++) {
    const tr = document.createElement("tr")
    for (x = 0; x < width; x++) {
      const td = document.createElement("td")
      const item = list[y * width + x]
      if (typeof item === "string") {
        td.innerText = item
      } else {
        td.innerHTML = `<strong>${item.text}<div class="freespace">${item.description}</div></strong>`
        td.classList.add("won")
      }
      td.addEventListener("click", toggleCell)
      tr.appendChild(td)
    }
    tbody.appendChild(tr)
  }
  table.appendChild(tbody)
  form.replaceWith(table)
}

const toggleCell = e => {
  e.currentTarget.classList.toggle("won")
  save()
}

const load = () => {
  if (!saveload) { return }
  const saved = localStorage.getItem(localstoragekey)
  if (!saved) { return }
  const keys = saved.split(",")
  document.querySelectorAll("td").forEach(cell => {
    if (keys.find(key => key === cell.innerText)) {
      cell.classList.add("won")
    }
  })
}
const save = () => {
  if (!saveload) { return }
  const keys = []
  document.querySelectorAll("td").forEach(cell => {
    if (cell.classList.contains("won")) {
      keys.push(cell.innerText)
    }
  })
  localStorage.setItem(localstoragekey, keys.join(","))
}

let allwords = []
list.forEach(item => {
  const words = item.match(/(\w{4,})/g)
  if (words) {
    allwords = allwords.concat(words)
  }
})
document.getElementById("seed").placeholder = allwords[Math.floor(Math.random() * allwords.length)]
allwords = []

const load_data_from_qs = () => {
  const urlParams = new URLSearchParams(window.location.search)

  seed = urlParams.get("seed")
  if (seed !== null) {
    loadseed(false)
  }
}
load_data_from_qs()
</script>"""