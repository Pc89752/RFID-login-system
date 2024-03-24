/**
 *
 * @param {SubmitEvent} event
 * @returns
 */
function checkStudentId(event) {
  event.preventDefault();
  const fd = new FormData(event.currentTarget);
  //   var studentId = document.getElementById("student_id").value;
  //   if (!/^[U][0-9]{8}$/.test(studentId)) {
  //     alert("學號必須是 9 位數，且第一位字母必須是大寫的 U");
  //     return false;
  //   }

  fetch("/register", { method: "POST", body: fd })
    .then((res) => res.json())
    .then((json) => {
        if(json.success)
            alert(`註冊成功:\n學號:${json.student_id}`)
        else alert("不可重複註冊學生證");
    });

  //   if ( === "true") {
  //     alert("不可重複註冊學生證");
  //     return false;
  //   }
  return true;
}
