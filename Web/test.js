const {Splitpanes, Pane} = window.splitpanes;

Vue.use(BootstrapVue, {
  BFormGroup: { labelCols: 3, labelAlign: "right", labelSize: "sm", class: "mb-1" },
  BFormInput: { size: 'sm' },
  BFormRadioGroup: { size: 'sm', plain: true },
  BFormSelect: { size: 'sm', plain: true },
  BButton: { size: 'sm' },
  BFormDatepicker: { size: 'sm', buttonOnly: true, buttonVariant: "", right: true, hideHeader: true }
});
Vue.use(VueCleave);

const items = [
  { trade: { ccy: "USD", expiry: "2022/1/22", strike: 101.636, type: "Call" }, isChecked: true },
  { trade: { ccy: "EUR", expiry: "2022-01-23", strike: 102.834, type: "Put" } },
  { trade: { ccy: "AUD", expiry: "2022-01-22", strike: 111.455, type: "Call", notional: 12345678 } },
  { trade: { ccy: "USD", expiry: "2022-01-22", strike: 121.987, type: "Call" }, isChecked: true },
  { trade: { ccy: "EUR", expiry: "2022-01-23", strike: 102.23, type: "Put" } },
  { trade: { ccy: "AUD", expiry: "2022-1-22", strike: 131.23, type: "Call" } },
  { trade: { ccy: "USD", expiry: "2022-1-22", strike: 101.23, type: "Call" } },
  { trade: { ccy: "EUR", expiry: "2022-1-23", strike: 132.23, type: "Put" } },
  { trade: { ccy: "AUD", expiry: "2022-1-22", strike: 101.23, type: "Call" } },
]
const master = {
  ccys: ["USD", "EUR", "AUD"],
  types: ["Call", "Put"]
}

const vm = new Vue({
  el: '.app',
  components: { Splitpanes, Pane },
  data: {
    items: [],
    cleave: {
      uint: {
        numeral: true,
        numeralPositiveOnly: true,
        numeralDecimalScale: 2
        //numeralIntegerScale: 0,
      },
    },
    fields: [
      { key: 'index', label: '', thStyle: { width: "30px" }, thClass: "text-right", tdClass: "text-right" },
      { key: 'radio', label: '', thStyle: { width: "30px" } },
      { key: 'checked', label: '', thStyle: { width: "30px" } },
      { key: 'type', label: 'Type', sortable: true, thStyle: { width: "70px" } },
      { key: 'ccy', sortable: true, thStyle: { width: "80px" } },
      { key: 'expiry', sortable: true, thStyle: { width: "125px" } },
      { key: 'strike', sortable: true, thStyle: { width: "120px" }, class: "dps-option" },
      { key: 'notional', sortable: true, thStyle: { width: "120px" } },
      'premium',
      'delta',
      'gamma',
      'vega',
    ],
    allcheck: false,
    //selectedItems: [],
    checkedIndex: 0,
    master: null,
    status: {
      isRunning: false
    },
  },
  created: function(){
     this.master = master;
     this.items = items;
  },
  watch: {
    "form.tenor": function(val) {
      //var conv = this.master.conventions.find(conv => conv.index == val);
      //this.form.dcc = conv.swapdcc;
    },
    allcheck: function(value) {
      this.items.forEach(function(item) {
        item.isChecked = value;
      });
    },
    items: function() {
      this.checkedIndex = this.checkedIndex < this.items.length ? this.checkedIndex : 0
    },
  },
  computed: {
    current: function() {
      return this.items[this.checkedIndex] || {};
    },
    selectedItems: function() {
      return this.items.filter(function(item) {
        return item.isChecked;
      });
    },
  },
  methods: {
    onSubmit: function(event) {
      var _this = this;
      this.result = {};
      this.status.isRunning = true;
      setTimeout(function() {
        _this.status.isRunning = false;
        _this.result = { premium: _this.form.strike + 1, delta: _this.form.strike + 2, greeks : { gamma: { "JPY": _this.form.strike + 3 } } }
        //alert(JSON.stringify(_this.form));
      }, 1000);
    },
    onReset: function(event) {
       this.current.trade = { bcs: [] };
       this.current.result = null;
    },
    round: function(val, digit) {
       var d = Math.pow(10, digit);
       return val ? Math.round(val * d) / d : val;
    },
    getFirst: function(obj) {
       return obj ? obj[Object.keys(obj)[0]] : null;
    },
    toggleAll: function(isChecked) {
      this.items.forEach(function(item) {
        //item.isChecked = isChecked;
      });
    },
    onAdd: function() {
        this.items.push({ ccy: "AUD", expiry: "2022-1-22", strike: 101.23, type: "Call" });
    },
    onDelete: function() {
        if (!confirm("delete?")) {
          return;
        }
        var items = this.items;
        (this.selectedItems || [this.current]).forEach(function(item) {
           items.splice(items.indexOf(item), 1);
        });
        if (items.length == 0) {
          onAdd();
        }
    },
    onClear: function() {
        if (!confirm("clear?")) {
          return;
        }
        this.items.splice(0);
        this.onAdd();
    },
    focusNext: function(event) {
      focusNext(event);
    },
    onLoad: function(event) {
      var _this = this;
      this.status.isRunning = true;
      setTimeout(function() {
        _this.status.isRunning = false;
        _this.form = {
          date: null,
          number: 123.45,
          asof: "2020-01-03",
          date: "2020-01-02",
          rate: 0.123,
          index: "JPY.LIBOR.3M",
          bcs: ["LDN", "TKY"]
        };
      }, 1000);
    }
  }
});


function onNumberKey(obj, name) {
  switch(event.key) {
    case "y":
    case "b":
      obj[name] *= 1000;
    case "m":
      obj[name] *= 1000;
    case " ":
      obj[name] *= 1000;
      event.preventDefault();
  }
}
function focusNext(event) {
  var inputs = event.target.form.querySelectorAll("input,select");
  var next = inputs[Array.prototype.indexOf.call(inputs, event.target) + 1];
  if (next) {
    next.focus();
  }
}

/*
window.onload = function() {
  document.querySelectorAll(window.location.hash.replace("?", ".")).forEach(function(item) {
    //item.style.display = "none";
  });
}
*/

    /*
    options: {
      date: {
        date: true,
        delimiter: '-',
        datePattern: ['Y', 'm', 'd'],
      },
      decimal: {
        numeral: true,
        //numeralIntegerScale: 5,
        numeralDecimalScale: 5,
      }
    },
    */

/*
var cleave = new Cleave('.uint', {
    numeral: true,
    //numeralThousandsGroupStyle: 'thousand',
    numeralPositiveOnly: true
});

var cleave = new Cleave('.input-numeric', {
    numeral: true,
    numeralThousandsGroupStyle: 'thousand',
    noImmediatePrefix: true,
    rawValueTrimPrefix: true,
        numeralIntegerScale: 9,
        numeralDecimalScale: 2
});
new Cleave('.input-date', {
    date: true,
        delimiter: '-',
        datePattern: ['Y', 'm', 'd'],
});
*/

