// Initialize select2 for better dropdown experience
$(document).ready(function () {
    // Category is already selected from previous page (SelectCategory)
    // CategoryId is passed as hidden field, no need for select2
    
    // Trigger attribute loading on page load if category is already set
  const categoryId = $('#categoryIdField').val();
    console.log('Category ID:', categoryId); // Debug log
    if (categoryId) {
 loadCategoryAttributes(categoryId);
    }

    // Initialize provinces select2 with simple mode
    $('#provinceSelect').select2({
        placeholder: 'Chọn tỉnh/thành phố',
        allowClear: true,
        width: '100%'
    });

    // Initialize condition select2
  $('#conditionSelect').select2({
        placeholder: 'Chọn tình trạng',
  allowClear: true,
        width: '100%'
    });

    // Function to load category attributes
    function loadCategoryAttributes(categoryId) {
    if (!categoryId) return;
        
        console.log('Loading attributes for category:', categoryId); // Debug log
        
        $.get(`/api/categories/${categoryId}/attributes`, function (data) {
console.log('Attributes loaded:', data); // Debug log
            const container = $('#additionalAttributes');
          container.empty();

          if (data && data.length > 0) {
                const heading = $('<h4>').text('Thông tin chi tiết').css('margin-top', '20px');
          container.append(heading);

         data.forEach(attr => {
      console.log('Generating field for:', attr); // Debug log
         const fieldHtml = generateAttributeField(attr);
         if (fieldHtml) {
       container.append(fieldHtml);
  }
       });

   // Re-initialize any special inputs (like select2)
    container.find('select').select2({
       width: '100%'
       });
            } else {
        console.log('No attributes found for this category'); // Debug log
      }
        }).fail(function(xhr, status, error) {
            console.error('Failed to load category attributes:', error); // Debug log
            console.error('Response:', xhr.responseText);
  });
    }

    // Image preview handling
    const imagePreview = document.getElementById('imagePreview');
  const imageInput = document.getElementById('imageInput');

 if (imageInput) {
        imageInput.addEventListener('change', function () {
imagePreview.innerHTML = '';
            const files = Array.from(this.files);
      
   if (files.length > 10) {
              alert('Chỉ được chọn tối đa 10 ảnh');
       this.value = '';
return;
  }

 files.forEach((file, index) => {
   if (file.size > 5 * 1024 * 1024) {
    alert('Mỗi ảnh không được quá 5MB');
          this.value = '';
           return;
           }

     const reader = new FileReader();
 reader.onload = function (e) {
         const div = document.createElement('div');
            div.className = 'preview-item';
               div.setAttribute('data-index', index);
         
         const isMain = index === 0;
     const starClass = isMain ? 'fas fa-star' : 'far fa-star';
             
 div.innerHTML = `
   <img src="${e.target.result}" alt="preview">
        <button type="button" class="set-main-image" title="Đặt làm ảnh chính">
         <i class="${starClass}"></i>
        </button>
    <button type="button" class="remove-image" title="Xóa ảnh">
      <i class="fas fa-times"></i>
        </button>
  `;
         imagePreview.appendChild(div);
           };
   reader.readAsDataURL(file);
   });
        });
    }

    // Set main image
    $(document).on('click', '.set-main-image', function () {
        $('.set-main-image i').removeClass('fas').addClass('far');
     $(this).find('i').removeClass('far').addClass('fas');
      
        const index = $(this).closest('.preview-item').attr('data-index');
$('#mainImageIndex').val(index);
    });

    // Remove image preview
  $(document).on('click', '.remove-image', function () {
        $(this).closest('.preview-item').remove();
        
   // Reset file input if all previews are removed
        if ($('.preview-item').length === 0) {
            imageInput.value = '';
    $('#mainImageIndex').val('0');
        } else {
       // Re-index remaining items
   $('.preview-item').each(function(index) {
      $(this).attr('data-index', index);
        });
            
 // Make sure first image is marked as main if current main was removed
            if ($('.set-main-image i.fas').length === 0) {
      $('.preview-item').first().find('.set-main-image i').removeClass('far').addClass('fas');
       $('#mainImageIndex').val('0');
            }
        }
    });

    // Add smooth scroll on validation error
    $('input, textarea, select').on('invalid', function(e) {
        e.preventDefault();
        $('html, body').animate({
            scrollTop: $(this).offset().top - 100
        }, 500);
        $(this).addClass('shake');
        setTimeout(() => $(this).removeClass('shake'), 500);
    });

    // Form submission - collect additional attributes
    $('#createPostForm').on('submit', function(e) {
        // Show loading state on button
        const submitBtn = $(this).find('button[type="submit"]');
        const originalText = submitBtn.html();
        submitBtn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Đang xử lý...');
   // Collect additional attributes into hidden fields
      const attributes = [];
        $('#additionalAttributes').find('[name^="AdditionalAttributes"]').each(function() {
            const input = $(this);
    const attrId = input.attr('name').match(/\[(\d+)\]/)[1];
      const attrType = input.data('attr-type') || 'text';
 const value = input.val();
    
         if (value) {
      const attr = {
          CategoryAttributeId: parseInt(attrId)
                };
                
                switch(attrType) {
          case 'number':
       attr.NumberValue = parseFloat(value);
           break;
   case 'select':
          attr.SelectValues = value;
      break;
                case 'boolean':
attr.BooleanValue = value === 'true';
           break;
                  case 'date':
         case 'datetime':
             attr.DateValue = value;
               break;
               default:
   attr.TextValue = value;
          }
    
                attributes.push(attr);
         }
        });
        
        // Add attributes as JSON to a hidden field
        if (attributes.length > 0) {
            attributes.forEach((attr, index) => {
        Object.keys(attr).forEach(key => {
          const hiddenField = $('<input>')
        .attr('type', 'hidden')
            .attr('name', `Request.Post.AttributeValues[${index}].${key}`)
            .val(attr[key]);
 $(this).append(hiddenField);
 });
    });
        }
        
        // Note: Button will be re-enabled on page reload or redirect
        // If there's an error, you may want to handle it in your response
    });

    // Add input animations
    $('.form-control').on('focus', function() {
        $(this).closest('.form-group').addClass('focused');
    }).on('blur', function() {
        $(this).closest('.form-group').removeClass('focused');
    });

    // Character counter for title and description
    const titleInput = $('input[name="Request.Post.Title"]');
    const descInput = $('textarea[name="Request.Post.Description"]');
    
    if (titleInput.length) {
        titleInput.after('<small class="char-count"></small>');
        titleInput.on('input', function() {
            const count = $(this).val().length;
            $(this).next('.char-count').text(`${count} ký tự`);
        });
    }
    
    if (descInput.length) {
        descInput.after('<small class="char-count"></small>');
        descInput.on('input', function() {
            const count = $(this).val().length;
            $(this).next('.char-count').text(`${count} ký tự`);
        });
    }
});

function generateAttributeField(attribute) {
    const requiredAttr = attribute.required ? 'required' : '';
    const requiredLabel = attribute.required ? '<span class="required">*</span>' : '';
    const placeholder = attribute.placeholder || '';
    
    console.log('Generating field - Type:', attribute.type, 'Name:', attribute.name); // Debug log
    
    switch (attribute.type) {
        case 'text':
            return `
     <div class="form-group">
        <label>${attribute.name} ${requiredLabel}</label>
      <input type="text" 
     name="AdditionalAttributes[${attribute.id}]" 
           data-attr-type="text"
      class="form-control" 
             placeholder="${placeholder}"
   ${requiredAttr}>
           </div>`;
        
        case 'select':
    const options = attribute.options?.map(opt => 
    `<option value="${opt.value}">${opt.label}</option>`).join('') || '';
            return `
       <div class="form-group">
         <label>${attribute.name} ${requiredLabel}</label>
   <select name="AdditionalAttributes[${attribute.id}]" 
        data-attr-type="select"
     class="form-control" 
             ${requiredAttr}>
           <option value="">Chọn ${attribute.name.toLowerCase()}</option>
     ${options}
  </select>
         </div>`;
        
    case 'number':
    return `
                <div class="form-group">
            <label>${attribute.name} ${requiredLabel}</label>
         <input type="number" 
 name="AdditionalAttributes[${attribute.id}]" 
         data-attr-type="number"
    class="form-control" 
      placeholder="${placeholder}"
         ${requiredAttr}>
                </div>`;

        case 'textarea':
     return `
           <div class="form-group">
        <label>${attribute.name} ${requiredLabel}</label>
           <textarea name="AdditionalAttributes[${attribute.id}]" 
    data-attr-type="text"
       class="form-control" 
       rows="3"
                placeholder="${placeholder}"
     ${requiredAttr}></textarea>
             </div>`;
        
        case 'email':
   return `
   <div class="form-group">
     <label>${attribute.name} ${requiredLabel}</label>
     <input type="email" 
         name="AdditionalAttributes[${attribute.id}]" 
        data-attr-type="text"
   class="form-control" 
       placeholder="${placeholder}"
        ${requiredAttr}>
          </div>`;

        case 'tel':
            return `
              <div class="form-group">
   <label>${attribute.name} ${requiredLabel}</label>
        <input type="tel" 
                   name="AdditionalAttributes[${attribute.id}]" 
      data-attr-type="text"
            class="form-control" 
       placeholder="${placeholder}"
  ${requiredAttr}>
            </div>`;
        
        case 'url':
   return `
          <div class="form-group">
        <label>${attribute.name} ${requiredLabel}</label>
          <input type="url" 
      name="AdditionalAttributes[${attribute.id}]" 
   data-attr-type="text"
 class="form-control" 
    placeholder="${placeholder}"
      ${requiredAttr}>
                </div>`;
     
        case 'date':
         return `
    <div class="form-group">
          <label>${attribute.name} ${requiredLabel}</label>
 <input type="date" 
  name="AdditionalAttributes[${attribute.id}]" 
               data-attr-type="date"
    class="form-control" 
              ${requiredAttr}>
   </div>`;
   
        case 'datetime':
            return `
     <div class="form-group">
     <label>${attribute.name} ${requiredLabel}</label>
     <input type="datetime-local" 
    name="AdditionalAttributes[${attribute.id}]" 
     data-attr-type="datetime"
     class="form-control" 
                ${requiredAttr}>
     </div>`;
        
    case 'boolean':
      return `
       <div class="form-group">
                    <label>${attribute.name} ${requiredLabel}</label>
           <select name="AdditionalAttributes[${attribute.id}]" 
        data-attr-type="boolean"
        class="form-control" 
     ${requiredAttr}>
       <option value="">Chọn</option>
     <option value="true">Có</option>
          <option value="false">Không</option>
              </select>
  </div>`;
 
        default:
 console.warn('Unknown attribute type:', attribute.type); // Debug log
    return '';
    }
}